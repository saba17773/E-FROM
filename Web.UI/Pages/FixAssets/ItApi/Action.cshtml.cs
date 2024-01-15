using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.FixAssets.ItApi
{
    public class ActionModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public string noncekey { get; set; }
        public List<SelectListItem> CompanyMaster { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ActionModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          ICreditControlService creditControlService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _creditControlService = creditControlService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(string nonce,string employee, string emailid, int compcode)
        {
            try
            {
                CompanyMaster = await GetCompanyMasterAsync();
                
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var dataResult = await unitOfWork.Transaction.Connection.QueryAsync<UserITOnlineModel>($@"
                        EXEC APIITCheckUser @nonce,@employee
                    ",
                    new
                    {
                        @nonce = nonce,
                        @employee = employee
                    }, unitOfWork.Transaction);
                    
                    var nonceItRepo = new GenericRepository<NonceITTable>(unitOfWork.Transaction);
                    var Result = "";
                    foreach (var item in dataResult)
                    {
                        Result = item.Result;
                    }
                    
                    if(Result!="HAVE"){
                        // Console.WriteLine(Result);
                        throw new Exception("Authentication it-online not found.");
                    }else{
                        //checknonce
                        //var nonceItAll = await nonceItRepo.GetAllAsync();
                        //var nonceIt = nonceItAll.Where(x =>
                        //    x.NonceKey == nonce &&
                        //    x.EmployeeId == employee &&
                        //    x.IsUsed == 0 &&
                        //    x.ExpireDate >= DateTime.Now)
                        //    .ToList();

                        //if (nonceIt.Count == 0)
                        //{
                        //    await nonceItRepo.InsertAsync(new NonceITTable
                        //    {
                        //        NonceKey = nonce,
                        //        EmployeeId = employee,
                        //        CreateDate = DateTime.Now,
                        //        ExpireDate = DateTime.Now.AddDays(1),
                        //        IsUsed = 0
                        //    });
                        //    unitOfWork.Complete();
                        //    // return Page();
                        //    // throw new Exception("Authentication not found.");
                        //    return Redirect("/FixAssets/"+nonce);
                        //}else{
                        //    noncekey = nonceIt[0].NonceKey;
                        //    return Redirect("/FixAssets/"+noncekey);
                        //    // return Page();
                        //}
                        
                        var users = await userRepo.GetAllAsync();

                        var user = users
                            .Where(x =>
                                x.EmployeeId == employee.Trim() &&
                                x.IsActive == 1)
                            .FirstOrDefault();

                        if (user == null)
                        {
                            //throw new Exception("Username or Password incorrect.");

                            var salt = Guid.NewGuid();
                            await userRepo.InsertAsync(new UserTable
                            {
                                Username = employee,
                                Password = "qC6u/yh2FYXvFT2Gwnuw2W+UIvNDg0MlffE4Akv7JYI=",
                                Salt = salt.ToString(),
                                Email = emailid,
                                RoleId = 20,
                                IsActive = 1,
                                UserDomain = emailid.Substring(0, emailid.IndexOf("@")),
                                EmployeeId = employee,
                                CompanyGroup = compcode
                            });
                            var usersnew = await userRepo.GetAllAsync();

                            var usernew = usersnew
                                .Where(x =>
                                    x.EmployeeId == employee.Trim() &&
                                    x.IsActive == 1)
                                .FirstOrDefault();

                            string tokennew = _authService.GenerateToken(usernew);
                            unitOfWork.Complete();
                            Response.Cookies.Append(_configuration["Jwt:Name"], tokennew, new CookieOptions()
                            {
                                Expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict
                            });

                            AlertSuccess = "Login Success.";

                            return Redirect("/FixAssets");
                        }
                        else
                        {
                            string token = _authService.GenerateToken(user);
                            unitOfWork.Complete();
                            Response.Cookies.Append(_configuration["Jwt:Name"], token, new CookieOptions()
                            {
                                Expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict
                            });

                            AlertSuccess = "Login Success.";

                            return Redirect("/Index");
                        }

                    }
                
                }

                //return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/FixAssets");
            }
        }

        public async Task<List<SelectListItem>> GetCompanyMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var companyRepo = new GenericRepository<Company>(unitOfWork.Transaction);

                var companyAll = await companyRepo.GetAllAsync();

                return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.CompanyId.ToString(),
                        Text = x.CompanyId,
                    })
                    .ToList();
            }
        }

    }
}