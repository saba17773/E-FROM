using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class RequestStatusModel
    {
        public static int Open { get; set; } = 1;
        public static int Cancel { get; set; } = 2;
        public static int WaitingForApprove { get; set; } = 3;
        public static int Reject { get; set; } = 4;
        public static int Complete { get; set; } = 5;
        public static int ReleaseVender { get; set; } = 6;
        public static int Successfully { get; set; } = 7;
        public static int Draft { get; set; } = 8;
        public static int MoreDetail { get; set; } = 9;
        public static int isNotPass { get; set; } = 10;
    }
}