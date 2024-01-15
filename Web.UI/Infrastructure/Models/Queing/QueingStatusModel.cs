using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Queing
{
    public class QueingStatusModel
    {
        public static int WaittoWeight { get; set; } = 1;
        public static int LoadComplete { get; set; } = 2;
        public static int CheckOut { get; set; } = 3;
        public static int Cancel { get; set; } = 4;
    }
}
