using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.CreditControl
{
    public class FormCreditControlInfo_DOM
    {
        public string CreditLimited { get; set; }
        public string CreditRating { get; set; }
        public int GuaranteeDOM_Check { get; set; }
        public int GuaranteeDOM_BGTotal { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GuaranteeDOM_IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GuaranteeDOM_ExpireDate { get; set; }
        public string GuaranteeDOM_Other { get; set; }
        public int TermOfPayment { get; set; }
    }
}
