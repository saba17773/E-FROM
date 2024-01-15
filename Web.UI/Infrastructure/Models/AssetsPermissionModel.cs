using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class AssetsPermissionModel
    {
        public static string VIEW_ASSETS { get => "View Transactions"; }
        public static string ADD_ASSETS { get => "Add Assets"; }
        public static string APPROVE_MASTER_ASSETS { get => "Approve Master"; }
        public static string APPROVE_MAPPING_ASSETS { get => "Approve Mapping"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(AssetsPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(AssetsPermissionModel)).ToString();
            }

            return "";
        }
    }
}
