using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
  [Table("TB_CreditControl")]
  public class CreditControlOVSTable
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
    public string Currency { get; set; }
    public string DeliveryCondition { get; set; }
    public string DestinationPort { get; set; }
    public string CreditLimited { get; set; }
    public int? GuaranteeOVS_Check { get; set; }
    public string GuaranteeOVS_StandbyLCAmount { get; set; }

    [DataType(DataType.Date)]
    public DateTime? GuaranteeOVS_IssueDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? GuaranteeOVS_ExpireDate { get; set; }
    public string GuaranteeOVS_SecurityDepositAmount { get; set; }
    public string GuaranteeOVS_Other { get; set; }
    public string CommentFromSale { get; set; }
    public string CommentFromApproval { get; set; }
    public int CurrentApproveStep { get; set; }
    public int CreateBy { get; set; }
    public DateTime CreateDate { get; set; }
    public int? UpdateBy { get; set; }
    public DateTime? UpdateDate { get; set; }

  }
}
