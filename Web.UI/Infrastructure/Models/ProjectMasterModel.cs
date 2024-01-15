using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ProjectMasterModel
    {
        public static int KPISystem { get; set; } = 1;
        public static int Queing { get; set; } = 2;
        public static int CreditControl { get; set; } = 3;
        public static int Memo { get; set; } = 4;
        public static int S2E { get; set; } = 5;
        public static int Promotion { get; set; } = 6;
    }
}
