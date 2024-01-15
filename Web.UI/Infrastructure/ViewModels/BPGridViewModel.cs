using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class BPGridViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int VersionId { get; set; }
        public string Version { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string CustGroup { get; set; }
        public string Quarter { get; set; }
        public string Remark { get; set; }
        public int Type { get; set; }
        public string SetType { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public string CreateName { get; set; }
        public string CreateDate { get; set; }
        public string UpdateName { get; set; }
        public string UpdateDate { get; set; }
        public double QTY { get; set; }
        public double Amount { get; set; }
        // public double Qty_01 { get; set; }
        // public double Amount_01 { get; set; }
        // public double Qty_02 { get; set; }
        // public double Amount_02 { get; set; }
        // public double Qty_03 { get; set; }
        // public double Amount_03 { get; set; }
        // public double Qty_Current { get; set; }
        // public double Amount_Current { get; set; }
        // public double Qty_Next { get; set; }
        // public double Amount_Next { get; set; }
    }
}