using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class RolePermissionModel
    {
        public static string VIEW_ROLE { get => "View"; }
        public static string ADD_ROLE { get => "Add"; }
        public static string EDIT_ROLE { get => "Edit"; }
        public static string REMOVE_ROLE { get => "Remove"; }
        public static string COPY_ROLE { get => "Copy Role"; }
        public static string VIEW_PERMISSION { get => "Update Permission"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(RolePermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(RolePermissionModel)).ToString();
            }

            return "";
        }
    }
}
