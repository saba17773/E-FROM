using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class SODGridViewModel
    {
        public string AllocateMonth { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Itemid { get; set; }
        public string Custcode { get; set; }
        public string Username { get; set; }
        public string CreateName { get; set; }
        public string CreateDate { get; set; }
        public double RequestQTY { get; set; }
        public double ConfirmQTY { get; set; }
        public double Cango { get; set; }
        public double Out { get; set; }
    }
}