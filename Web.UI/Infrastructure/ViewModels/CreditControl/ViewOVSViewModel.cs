using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.CreditControl
{
  public class ViewOVSViewModel
  {
    public string RequestNumber { get; set; }

    [DataType(DataType.Date)]
    public string RequestDate { get; set; }
    public string CustomerCode { get; set; }
    public string TypeOfBusiness { get; set; }
    public string TypeOfProduct { get; set; }
    public string CompanyName { get; set; }
    public string IsHeadQuarter { get; set; }
    public string Branch { get; set; }
    public string AddressNo { get; set; }
    public string Street { get; set; }
    public int ZipCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string ContactPerson1 { get; set; }
    public string ContactPerson1_Tel { get; set; }
    public string ContactPerson1_Email { get; set; }
    public string ContactPerson2 { get; set; }
    public string ContactPerson2_Tel { get; set; }
    public string ContactPerson2_Email { get; set; }

    // sale info
    public string SaleName { get; set; }
    public string SaleZone { get; set; }
    public string DeliveryCondition { get; set; }
    public string DimensionNo { get; set; }
    public string DiscountByConstant { get; set; }
    public string DiscountByStep { get; set; }
    public string TermOfDelivery { get; set; }
    public string TermOfDelivery_Other { get; set; }
    public string Currency { get; set; }
    public string DestinationPort { get; set; }

    public string CreditLimited { get; set; }
    public string CreditRating { get; set; }
    public string GuaranteeOVS_Check { get; set; }
    public string GuaranteeOVS_IssueDate { get; set; }
    public string GuaranteeOVS_ExpireDate { get; set; }
    public string GuaranteeOVS_SecurityDepositAmount { get; set; }
    public string GuaranteeOVS_StandbyLCAmount { get; set; }
    public string GuaranteeOVS_Other { get; set; }
    public string CommentFromSale { get; set; }
  }
}