using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.CreditControl
{
    public class FormAddDOMViewModel
    {
        public int RequestNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }
        public string CustomerCode { get; set; }
        public int TypeOfBusiness { get; set; }
        public string CompanyName { get; set; }
        public int IsHeadQuarter { get; set; }
        public string Branch { get; set; }

        public string SaleEmployeeId { get; set; }
        public string SaleZone { get; set; }
        public string DimensionNo { get; set; }
        public string DiscountByConstant { get; set; }
        public string DiscountByStep { get; set; }
        public string TermOfDelivery { get; set; }
        public string TermOfDelivery_Other { get; set; }
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
        public string CommentFromSale { get; set; }
        public string CommentFromApproval { get; set; }
    }
}
