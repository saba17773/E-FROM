using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class TicketPermissionModel
    {
        public static string VIEW_MASTER_BP { get => "Master BP"; }
        public static string VIEW_MASTER_FC { get => "Master FC"; }
        public static string VIEW_MASTER_SOD { get => "Master SOD For AOT"; }
        public static string VIEW_IMPORT_TEMPLATE_BP { get => "View Import Template BP"; }
        public static string VIEW_IMPORT_TEMPLATE_FC_OVS { get => "View Import Template FC OVS"; }
        public static string VIEW_IMPORT_TEMPLATE_FC_DOM { get => "View Import Template FC DOM+CLM"; }
        public static string VIEW_IMPORT_TEMPLATE_SOD { get => "View Import Template SOD For AOT"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(TicketPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(TicketPermissionModel)).ToString();
            }

            return "";
        }
    }
}
