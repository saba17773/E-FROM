using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class MemoItemTransViewModel
    {
        public int SALESID { get; set; }
        public string QuotationId { get; set; }
        public string EnquiryId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public string DATAAREAID { get; set; }
        
    }
}