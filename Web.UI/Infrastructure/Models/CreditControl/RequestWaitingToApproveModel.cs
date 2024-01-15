using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.CreditControl
{
    public class RequestWaitingToApproveModel
    {
        public int TransId { get; set; }
        public int CCId { get; set; }
        public string Email { get; set; }
        public string RequestNumber { get; set; }
        public string CompanyName { get; set; }
    }
}
