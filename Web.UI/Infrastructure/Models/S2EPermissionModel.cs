using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class S2EPermissionModel
    {
        public static string VIEW_S2E { get => "View S2E"; }
        public static string VIEW_PURCHASE { get => "View Purchase"; }
        public static string VIEW_QTECH { get => "View Qtech"; }
        public static string APPROVE_MAPPING_S2E { get => "S2E Approve Mapping"; }
        public static string APPROVE_MASTER_MANAGE { get => "S2E Approve Mater Manage"; }
        //New Permission
        public static string VIEW_ALLTRANSACTION { get => "View All Transaction"; }
        public static string VIEW_NEWREQUEST { get => "View New Request"; }
        public static string MANAGE_NEWREQUEST { get => "Manage New Request"; }
        public static string VIEW_RMASSESSMENT { get => "View ใบขอประเมินวัตถุดิบ"; }
        public static string MANAGE_RMASSESSMENT { get => "Manage ใบขอประเมินวัตถุดิบ"; }
        public static string VIEW_ADDRAWMATERIALSAMPLE { get => "View เพิ่มวัตถุดิบเข้าระบบ (SAMPLE)"; }
        public static string MANAGE_ADDRAWMATERIALSAMPLE { get => "Manage เพิ่มวัตถุดิบเข้าระบบ (SAMPLE)"; }
        public static string VIEW_RAWMATERIALREQUESTSAMPLE { get => "View ใบเบิกวัตถุดิบ (SAMPLE)"; }
        public static string MANAGE_RAWMATERIALREQUESTSAMPLE { get => "Manage ใบเบิกวัตถุดิบ (SAMPLE)"; }
        public static string VIEW_LABTEST{ get => "View Lab Test"; }
        public static string MANAGE_LABTEST { get => "Manage Lab Test"; }
        public static string VIEW_PURCHASESAMPLE { get => "View Purchase Sample"; }
        public static string MANAGE_PURCHASESAMPLE { get => "Manage Purchase Sample"; }
        public static string VIEW_ADDRAWMATERIAL { get => "View เพิ่มวัตถุดิบเข้าระบบ"; }
        public static string MANAGE_ADDRAWMATERIAL { get => "Manage เพิ่มวัตถุดิบเข้าระบบ"; }
        public static string VIEW_RAWMATERIALREQUEST { get => "View ใบเบิกวัตถุดิบ"; }
        public static string MANAGE_RAWMATERIALREQUEST { get => "Manage ใบเบิกวัตถุดิบ"; }
        public static string VIEW_TRIALTEST { get => "View Trial Test"; }
        public static string MANAGE_TRIALTEST { get => "Manage Trial Test"; }
        public static string VIEW_REPORT_STOCKCARD { get => "View Report Stock Card"; }


        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(S2EPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(S2EPermissionModel)).ToString();
            }

            return "";
        }
    }
}
