using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.CreditControl
{
  public class ViewDOMViewModel
  {
    public string RequestNumber { get; set; }
    public string RequestDate { get; set; }
    public string CustomerCode { get; set; }
    public string TypeOfBusiness { get; set; }
    public string TypeOfProduct { get; set; }
    public string CompanyName { get; set; }
    public string IsHeadQuarter { get; set; }
    public string Branch { get; set; }
    public string Address1_AddressNo { get; set; }
    public string Address1_Moo { get; set; }
    public string Address1_Soi { get; set; }
    public string Address1_Street { get; set; }
    public string Address1_Province { get; set; }
    public string Address1_District { get; set; }
    public string Address1_SubDistrict { get; set; }
    public string Address1_ZipCode { get; set; }
    public string Address1_Tel { get; set; }
    public string Address1_Fax { get; set; }
    public string Address1_Email { get; set; }
    public string Address2_AddressNo { get; set; }
    public string Address2_Moo { get; set; }
    public string Address2_Soi { get; set; }
    public string Address2_Street { get; set; }
    public string Address2_Province { get; set; }
    public string Address2_District { get; set; }
    public string Address2_SubDistrict { get; set; }
    public string Address2_ZipCode { get; set; }
    public string Address2_Tel { get; set; }
    public string Address2_Fax { get; set; }
    public string Address2_Email { get; set; }

    public string SaleName { get; set; }
    public string SaleZone { get; set; }
    public string DimensionNo { get; set; }
    public string DiscountByConstant { get; set; }
    public string DiscountByStep { get; set; }
    public string TermOfDelivery { get; set; }
    public string TermOfDelivery_Other { get; set; }

    public string CreditLimited { get; set; }
    public string CreditRating { get; set; }
    public string GuaranteeDOM_Check { get; set; }
    public string GuaranteeDOM_BGTotal { get; set; }
    public string GuaranteeDOM_IssueDate { get; set; }
    public string GuaranteeDOM_ExpireDate { get; set; }
    public string GuaranteeDOM_Other { get; set; }
    public string TermOfPayment { get; set; }
    public string CommentFromSale { get; set; }
  }
}
