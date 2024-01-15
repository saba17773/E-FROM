using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class VenderPermissionModel
    {
        public static string VIEW_VENDER { get => "View Transactions"; }
        public static string ADD_VENDER { get => "Add Vendor"; }
        public static string APPROVE_MAPPING_VENDER { get => "Vendor Approve Mapping"; }
        public static string MANAGE_VENDER { get => "Manage Vendor"; }
        public static string VIEW_VENDORTODOLIST { get => "View Vendor TodoList"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(VenderPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(VenderPermissionModel)).ToString();
            }

            return "";
        }

    }
}
