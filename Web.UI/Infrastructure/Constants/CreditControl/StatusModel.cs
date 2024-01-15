using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Constants.CreditControl
{
    public static class StatusModel
    {
        public enum RequestStatus
        { 
            Open = 1,
            Cancel = 2,
            WaitingForApprove = 3,
            Reject = 4,
            CompleteApprove = 5,
            ReleaseVender = 6,
            Successfully = 7,
            Draft = 8
        }
    }
}
