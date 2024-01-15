using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;

namespace Web.UI.Interfaces
{
    public interface IAuthService
    {
        string HashPassword(string password, string salt);
        string RandomPassword(int passwordLength);
        string GenerateToken(UserTable user);
        UserClaimsModel GetClaim();
        Task<bool> CanDisplay(string slug);
        bool CanDisplay(Type PermissionClass, string slug);
        Task CanAccess(string slug);
    }
}
