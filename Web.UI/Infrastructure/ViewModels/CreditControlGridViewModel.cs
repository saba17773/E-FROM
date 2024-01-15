using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class CreditControlGridViewModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string SaleName { get; set; }
        public string RequestType { get; set; }
        public string CompanyName { get; set; }
        public int RequestStatusId { get; set; }
        public string RequestStatus { get; set; }
        public int CurrentApproveStep { get; set; }
        public int TotalApproveStep { get; set; }
    }
}