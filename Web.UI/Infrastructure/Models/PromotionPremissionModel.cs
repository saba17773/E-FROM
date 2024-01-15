using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class PromotionPermissionModel
    {
        public static string VIEW_PROMOTION { get => "View Transactions"; }
        public static string APPROVE_MASTER_CREDITCONTROL { get => "Approve Master"; }
        public static string APPROVE_MAPPING_PROMOTION { get => "Approve Mapping"; }
        public static string ADD_DOM_PROMOTION { get => "Add DOM"; }
        public static string ADD_OVS_PROMOTION { get => "Add OVS"; }
        public static string ADD_EXP_DOM_PROMOTION { get => "Add Exceptional DOM"; }
        public static string ADD_EXP_OVS_PROMOTION { get => "Add Exceptional OVS"; }
        public static string VIEW_PROMOTION_COMPLETE { get => "View Transactions Complete"; }
        public static string VIEW_PROMOTION_CANCEL { get => "View Transactions Cancel"; }
        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(PromotionPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(PromotionPermissionModel)).ToString();
            }

            return "";
        }
    }
}
