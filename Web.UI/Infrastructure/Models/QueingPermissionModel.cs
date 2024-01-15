using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class QueingPermissionModel
    {
        public static string ADMIN_QUEING { get => "Admin Queing"; }
        public static string MASTERSETUP_QUEING { get => "Master Setup Queing"; }
        public static string USER_QUEING { get => "User Queing"; }
        public static string VIEW_QUEING { get => "View Queing Project"; }
        public static string ADD_QUEING { get => "Add Queing"; }
        public static string EDIT_QUEING { get => "Edit Queing"; }
        public static string CANCEL_QUEING { get => "Cancel Queing"; }
        public static string VIEW_QUEING_REPORT { get => "View Queing Report"; }
        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(QueingPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(QueingPermissionModel)).ToString();
            }

            return "";
        }
    }
}
