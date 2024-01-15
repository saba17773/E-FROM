using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Web.UI.Contexts;
using Web.UI.DataAccess.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Services
{
    public class AuthService : IAuthService
    {
        private IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private IDatabaseContext _databaseContext;
        private NewContext _newContext;

        public AuthService(
          IConfiguration configuration,
          IHttpContextAccessor httpContextAccessor,
          IDatabaseContext databaseContext,
          NewContext newContext)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _databaseContext = databaseContext;
            _newContext = newContext;
        }

        public async Task CanAccess(string slug)
        {
            try
            {
                var userCanDisplay = await CanDisplay(slug);
                if (userCanDisplay == false)
                {
                    throw new Exception("You can't access this section.");
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<bool> CanDisplay(string slug)
        {
            try
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    throw new Exception("Please login.");
                }

                var userClaims = GetClaim();

                if (userClaims == null)
                {
                    throw new Exception("User claims not found.");
                }

                int roleId = GetClaim().RoleId;
                int userId = GetClaim().UserId;
                string username = GetClaim().Username;
                string email = GetClaim().Email;
                string companygroup = GetClaim().CompanyGroup;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var permissionRepo = new GenericRepository<PermissionTable>(unitOfWork.Transaction);

                    var permissionAll = await permissionRepo.GetAllAsync();

                    var permission = permissionAll
                      .Where(x =>
                       x.RoleId == roleId &&
                       x.CapabilityId == slug)
                      .FirstOrDefault();

                    unitOfWork.Complete();

                    if (permission == null)
                    {
                        if (username == "administrator")
                        {
                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool CanDisplay(Type PermissionClass, string slug)
        {
            try
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    throw new Exception("Please login.");
                }

                var userClaims = GetClaim();

                if (userClaims == null)
                {
                    throw new Exception("User claims not found.");
                }

                int roleId = GetClaim()?.RoleId ?? 0;
                int userId = GetClaim()?.UserId ?? 0;
                string username = GetClaim()?.Username;
                string email = GetClaim()?.Email;

                if (username == "administrator")
                {
                    return true;
                }

                using (var context = _newContext)
                {
                    var permission = context.Permissions.Where(x => x.RoleId == roleId).ToList();

                    foreach (var item in permission)
                    {
                        if (item.PermissionId == slug)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GenerateToken(UserTable user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var _claims = new Claim[] {
                  new Claim ("id", user.Id.ToString ()),
                  new Claim ("role_id", user.RoleId.ToString ()),
                  new Claim ("username", user.Username),
                  new Claim ("employee_id", user.EmployeeId == null ? "" : user.EmployeeId),
                  new Claim (ClaimTypes.Email, user.Email),
                  new Claim ("companygroup", user.CompanyGroup.ToString()),
                };

                var token = new JwtSecurityToken(
                  _configuration["Config:BaseUrl"],
                  _configuration["Config:BaseUrl"],
                  _claims,
                  expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public UserClaimsModel GetClaim()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {

                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id")).Value;
                var roleId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("role_id")).Value;
                var username = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("username")).Value;
                var email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals(ClaimTypes.Email)).Value;
                var employeeId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("employee_id")).Value;
                var companygroup = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("companygroup")).Value;

                var claims = new UserClaimsModel
                {
                    UserId = Convert.ToInt32(userId),
                    RoleId = Convert.ToInt32(roleId),
                    Username = username,
                    Email = email,
                    EmployeeId = employeeId,
                    CompanyGroup = companygroup
                };

                return claims;
            }

            return new UserClaimsModel();
        }

        public string HashPassword(string password, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: password,
              salt: Encoding.ASCII.GetBytes(salt),
              prf: KeyDerivationPrf.HMACSHA1,
              iterationCount: 10000,
              numBytesRequested: 256 / 8));
            return hashed;
        }

        public string RandomPassword(int passwordLength)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";

            Random random = new Random();

            char[] chars = new char[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(chars);
        }
    }
}