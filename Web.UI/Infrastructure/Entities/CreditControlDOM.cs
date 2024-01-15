using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_CreditControl")]
    public class CreditControlDOMTable
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }
        public string CustomerCode { get; set; }

        [Required]
        public int? TypeOfBusiness { get; set; }
        public int? TypeOfProduct { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public int? IsHeadQuarter { get; set; }
        public string Branch { get; set; }
        public string RequestType { get; set; }
        public int RequestStatus { get; set; }
        public string SaleEmployeeId { get; set; }
        public string SaleZone { get; set; }
        public string DimensionNo { get; set; }
        public string DiscountByConstant { get; set; }
        public string DiscountByStep { get; set; }
        public string TermOfDelivery { get; set; }
        public string TermOfDelivery_Other { get; set; }
        public int TermOfPayment { get; set; }
        public string CreditLimited { get; set; }
        public string CreditRating { get; set; }
        public int? PaymentMethod { get; set; }
        public int? GuaranteeDOM_Check { get; set; }
        public float? GuaranteeDOM_BGTotal { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GuaranteeDOM_IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GuaranteeDOM_ExpireDate { get; set; }
        public string GuaranteeDOM_Other { get; set; }
        public string CommentFromSale { get; set; }
        public string CommentFromApproval { get; set; }

        [Required]
        public int CurrentApproveStep { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
