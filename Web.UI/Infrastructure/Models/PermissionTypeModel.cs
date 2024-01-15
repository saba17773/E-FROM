using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class PermissionTypeModel
    {
        public static string GENERAL { get; set; }
        public static string USER { get; set; }
        public static string ROLE { get; set; }
        public static string CREDIT_CONTROL { get; set; }

        public static string VENDER { get; set; }
        public static string IMPORT { get; set; }

        public static string APPROVEMASTER { get; set; }
        public static string PROMOTION { get; set; }
        public static string TICKET { get; set; }

        public static string S2E { get; set; }
        public static string MEMO { get; set; }
        public static string ASSETS { get; set; }
        public static string QUEING { get; set; }
        public static string COVID_TRACKER { get; set; }
    }
}
