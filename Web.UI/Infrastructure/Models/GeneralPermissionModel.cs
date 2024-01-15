using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class GeneralPermissionModel
    {
        public static string VIEW_DASHBOARD { get => "View Dashboard"; }
        public static string VIEW_PROJECTS { get => "View Projects"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(GeneralPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(GeneralPermissionModel)).ToString();
            }

            return "";
        }
    }
}
