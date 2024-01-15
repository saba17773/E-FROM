using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.CreditControl
{
    public class FormCreditControlInfo_OVS
    {
        public string CreditLimited { get; set; }
        public int GuaranteeOVS_Check { get; set; }
        public string GuaranteeOVS_StandbyLCAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GuaranteeOVS_IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GuaranteeOVS_ExpireDate { get; set; }
        public string GuaranteeOVS_SecurityDepositAmount { get; set; }
        public string GuaranteeOVS_Other { get; set; }
    }
}
