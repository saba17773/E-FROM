using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.CreditControl
{
    public class ApproveFlowPermissionModel
    {
        public static string VIEW_APPROVE_FLOW { get => "View Approve Flow"; }
        public static string ADD_APPROVE_FLOW { get => "Add Approve Flow"; }
        public static string EDIT_APPROVE_FLOW { get => "Edit Approve Flow"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(ApproveFlowPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(ApproveFlowPermissionModel)).ToString();
            }

            return "";
        }
    }
}
