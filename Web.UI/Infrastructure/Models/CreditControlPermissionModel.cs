using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class CreditControlPermissionModel
    {
        public static string VIEW_CREDITCONTROL { get => "View Transactions"; }
        public static string ADD_DOM_CREDITCONTROL { get => "Add DOM"; }
        public static string ADD_OVS_CREDITCONTROL { get => "Add OVS"; }
        public static string APPROVE_MASTER_CREDITCONTROL { get => "Approve Master"; }
        public static string APPROVE_FLOW_CREDITCONTROL { get => "Approve Flow"; }
        public static string APPROVE_MAPPING_CREDITCONTROL { get => "Approve Mapping"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(CreditControlPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(CreditControlPermissionModel)).ToString();
            }

            return "";
        }
    }
}
