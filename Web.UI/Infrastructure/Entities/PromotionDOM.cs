using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Promotion")]
    public class PromotionDOMTable
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }
        public string RequestType { get; set; }
        [Required]
        public int? TypeOfProduct { get; set; }
        public string Pattern { get; set; }
        [Required]
        public int TypeOf { get; set; }
        public string TypeOfRemark { get; set; }
        // [Required]
        public string CustomerName { get; set; }
        [Required]
        public int CustomerGroup { get; set; }
        public string CustomerGroupRemark { get; set; }
        public string PaymentType { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }
        [Required]
        public int TypeFrom { get; set; }
        public string TypeFromRemark { get; set; }
        public string Objective { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double SalesPresentBath { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double SalesForecastBath { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double SalesChangeBath { get; set; }
        public string SalesRemarkBath { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public Double SalesPresentQty { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public Double SalesForecastQty { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public Double SalesChangeQty { get; set; }
        public string SalesRemarkQty { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double BudgetPresent { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double BudgetForecast { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double BudgetChange { get; set; }
        public string BudgetRemark { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double BudgetPresentBath { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double BudgetForecastBath { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double? BudgetChangeBath { get; set; }
        public string BudgetRemarkBath { get; set; }
        public int GetDiscount { get; set; }
        public int GetPoint { get; set; }
        [Required]
        public string PromotionConditions { get; set; }
        public string MaketingRemark { get; set; }
        public string FARemark { get; set; }
        public int CurrentApproveStep { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int RequestStatus { get; set; }
        // [Required]
        public int PromotionRef { get; set; }
        public string CancelRemark { get; set; }
    }
}
