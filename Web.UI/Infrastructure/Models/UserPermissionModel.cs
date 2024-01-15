using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class UserPermissionModel
    {
        public static string VIEW_USER { get => "View"; }
        public static string ADD_USER { get => "Add"; }
        public static string EDIT_USER { get => "Edit"; }
        public static string REMOVE_USER { get => "Remove"; }
        public static string RESET_PASSWORD_USER { get => "Reset Password"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(UserPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey) 
                    return item.GetValue(typeof(UserPermissionModel)).ToString();
            }
            
            return "";
        }
    }
}
