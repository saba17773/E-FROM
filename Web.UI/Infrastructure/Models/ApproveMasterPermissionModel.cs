using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class ApproveMasterPermissionModel
    {
        public static string VIEW_APPROVE_MASTER { get => "View Approve Master"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(ApproveMasterPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(ApproveMasterPermissionModel)).ToString();
            }

            return "";
        }
    }
}
