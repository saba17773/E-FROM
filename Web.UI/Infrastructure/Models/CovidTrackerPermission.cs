using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class CovidTrackerPermission
    {
        public static string PROJECT_COVID_TRACKER { get => "โปรเจค Covid Tracker"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(CovidTrackerPermission).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(CovidTrackerPermission)).ToString();
            }

            return "";
        }
    }
}
