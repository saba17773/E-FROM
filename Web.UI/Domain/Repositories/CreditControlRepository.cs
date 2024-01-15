using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models.CreditControl;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Infrastructure.ViewModels.CreditControl;

namespace Web.UI.Domain.Repositories
{
  public class CreditControlRepository : ICreditControlRepository
  {
    private IDbTransaction _dbTransaction;

    public CreditControlRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public async Task<AddressTable> GetAddressByCCIdAsync(int ccId, string addressType)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<AddressTable>($@"
                SELECT * FROM TB_Address 
                WHERE CCId = '{ccId}'
                AND AddressType = '{addressType}'
            ", null, _dbTransaction);
    }

    public async Task<IEnumerable<ApprovalRemarkViewModel>> GetApprovalRemarkAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<ApprovalRemarkViewModel>($@"
                SELECT 
                AF.Name + ' ' + AF.LastName AS Name,
                AT.Email,
                AT.ApproveLevel,
                AT.Remark,
                AT.Urgent,
                AF.BackupEmail
                FROM TB_CreditControlApproveTrans AT
                LEFT JOIN TB_ApproveFlow AF ON AF.Id = AT.ApproveFlowId
                WHERE AT.CCId = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<ApproveFlowTable> GetApproveFlowBackupEmailAsync(int approveMasterId, int approveLevel)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ApproveFlowTable>($@"
                SELECT * FROM TB_ApproveFlow
                WHERE ApproveMasterId = {approveMasterId}
                AND ApproveLevel = {approveLevel} 
            ", null, _dbTransaction);
    }

    public async Task<IEnumerable<ApproveFlowTable>> GetApproveFlowByApproveMasterIdAsync(int id)
    {
      return await _dbTransaction.Connection.QueryAsync<ApproveFlowTable>($@"
                SELECT * FROM TB_ApproveFlow
                WHERE ApproveMasterId = {id}
            ", null, _dbTransaction);
    }

    public async Task<IEnumerable<ApproveFlowTable>> GetApproveFlowByCCIdAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<ApproveFlowTable>($@"
                SELECT 
                AF.*
                FROM TB_CreditControlApproveTrans CCAT
                LEFT JOIN TB_ApproveFlow AF ON AF.ApproveMasterId = CCAT.ApproveMasterId AND AF.ApproveLevel = CCAT.ApproveLevel
                LEFT JOIN TB_ApproveMaster AM ON AM.Id = AF.ApproveMasterId
                WHERE CCAT.CCId = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<CreditControlApproveMappingTable> GetApproveGroupId(string creditControlType, int createBy)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<CreditControlApproveMappingTable>($@"
                SELECT * 
                FROM TB_CreditControlApproveMapping 
                WHERE CCType = '{creditControlType}'
                AND CreateBy = {createBy}
            ", null, _dbTransaction);
    }

    public async Task<IEnumerable<CreditControlApproveTransTable>> GetApproveTransByCCId(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<CreditControlApproveTransTable>($@"
                SELECT *
                FROM TB_CreditControlApproveTrans
                WHERE CCId = {creditControlId}
            ", null, _dbTransaction);
    }

    public async Task<CreditControlApproveTransTable> GetApproveTransByLevel(int creditControlId, int level)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<CreditControlApproveTransTable>($@"
                SELECT *
                FROM TB_CreditControlApproveTrans
                WHERE CCId = {creditControlId}
                AND ApproveLevel = {level} 
            ", null, _dbTransaction);
    }

    public async Task<CreditControlAttachFileTable> GetAttachFileAsync(int id, string creditControlType, int fileNo)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<CreditControlAttachFileTable>($@"
                SELECT *
                FROM TB_CreditControlAttachFile
                WHERE CCId = {id}
                AND FileNo = {fileNo}
                AND CCType = {creditControlType}
            ", null, _dbTransaction);
    }

    public async Task<IEnumerable<CreditControlCheckListFileTable>> GetCheckListFileByCCIdAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<CreditControlCheckListFileTable>($@"
                SELECT *
                FROM TB_CreditControlCheckListFile
                WHERE CCId = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<IEnumerable<CustomerTypeByProductTransTable>> GetCustomerTypeTransByCCId(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<CustomerTypeByProductTransTable>($@"
                SELECT *
                FROM TB_CustomerTypeByProductTrans CTT
                WHERE CTT.CCId = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<IEnumerable<CustomerTypeTransViewModel>> GetCustomerTypeTransViewByCCId(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<CustomerTypeTransViewModel>($@"
                SELECT
                CP.Id,
                CTT.CCId,
                CP.ByName,
                CTT.CustomerByProductId,
                CTT.CustomerCode
                FROM TB_CustomerTypeByProduct CP
                LEFT JOIN TB_CustomerTypeByProductTrans CTT 
                ON CTT.CustomerByProductId = CP.Id AND CTT.CCId = @CCId
                GROUP BY
                CP.Id,
                CTT.CCId,
                CP.ByName,
                CTT.CustomerByProductId,
                CTT.CustomerCode
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<ViewDOMViewModel> GetDOMDataByCCIdAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ViewDOMViewModel>($@"


                Declare @val Varchar(MAX); 
                Select @val = COALESCE(@val + ', ' + CT.Description, CT.Description) 
                From TB_CustomerTypeByProductTrans  CTT 
                left join TB_CustomerType CT ON CT.Id = CTT.CustomerByProductId
                where CTT.CCId = @CCId;

                SELECT 
                CC.RequestNumber,
                CC.RequestDate,
                CC.CustomerCode,
                CC.CompanyName,
                CT.Description AS TypeOfBusiness,
                @val AS TypeOfProduct,
                CASE
                    WHEN CC.IsHeadQuarter = 1 THEN 'สาขาใหญ่'
                    ELSE 'สาขาย่อย'
                END AS IsHeadQuarter,
                CC.Branch,
                AD1.AddressNo AS Address1_AddressNo,
                AD1.Moo AS Address1_Moo,
                AD1.Soi AS Address1_Soi,
                AD1.Street AS Address1_Street,
                P1.NameInThai AS Address1_Province,
                D1.NameInThai AS Address1_District,
                S1.NameInThai AS Address1_SubDistrict,
                AD1.ZipCode AS Address1_ZipCode,
                AD1.Tel AS Address1_Tel,
                AD1.Fax AS Address1_Fax,
                AD1.Email AS Address1_Email,
                AD2.AddressNo AS Address2_AddressNo,
                AD2.Moo AS Address2_Moo,
                AD2.Soi AS Address2_Soi,
                AD2.Street AS Address2_Street,
                P2.NameInThai AS Address2_Province,
                D2.NameInThai AS Address2_District,
                S2.NameInThai AS Address2_SubDistrict,
                AD2.ZipCode AS Address2_ZipCode,
                AD2.Tel AS Address2_Tel,
                AD2.Fax AS Address2_Fax,
                AD2.Email AS Address2_Email,
                E.Name + ' ' + E.LastName AS SaleName,
                CC.SaleZone,
                CC.DimensionNo,
                CC.DiscountByConstant,
                CC.DiscountByStep,
                CASE 
                  WHEN CC.TermOfDelivery = 'NOT_PAY' THEN 'รับผิดชอบการส่งของถึงมือลูกค้า'
                  WHEN CC.TermOfDelivery = 'COMPANY_PAY' THEN 'ส่งของที่บริษัทขนส่ง/รับผิดชอบค่าขนส่ง'
                  WHEN CC.TermOfDelivery = 'CLIENT_PAY' THEN 'ส่งของที่บริษัทขนส่ง/ลูกค้าจ่ายค่าขนส่ง'
                  WHEN CC.TermOfDelivery = 'OTHER' THEN 'อื่นๆ โปรดระบุ'
                END AS TermOfDelivery,
                CC.TermOfDelivery_Other,
                CC.CreditLimited,
                CC.CreditRating,
                CASE 
                  WHEN CC.GuaranteeDOM_Check = 1 THEN 'B/G'
                  ELSE 'อื่นๆ'
                END AS GuaranteeDOM_Check,
                CC.GuaranteeDOM_BGTotal,
                CC.GuaranteeDOM_ExpireDate,
                CC.GuaranteeDOM_IssueDate,
                CC.GuaranteeDOM_Other,
                PT.Description AS TermOfPayment,
                CC.CommentFromSale
                FROM TB_CreditControl CC
                LEFT JOIN TB_Address AD1 ON AD1.CCId = CC.Id AND AD1.AddressType = 'Address1_DOM'
                LEFT JOIN TB_Address AD2 ON AD2.CCId = CC.Id AND AD2.AddressType = 'Address2_DOM'
                LEFT JOIN TB_Employee E ON E.EmployeeId = CC.SaleEmployeeId
                LEFT JOIN TB_Provinces P1 ON P1.Id = AD1.Province
                LEFT JOIN TB_Provinces P2 ON P2.Id = AD2.Province
                LEFT JOIN TB_Districts D1 ON D1.Id = AD1.District
                LEFT JOIN TB_Districts D2 ON D2.Id = AD2.District
                LEFT JOIN TB_Subdistricts S1 ON S1.Id = AD1.SubDistrict
                LEFT JOIN TB_Subdistricts S2 ON S2.Id = AD2.SubDistrict
                LEFT JOIN TB_CustomerType CT ON CT.Id = CC.TypeOfBusiness
                LEFT JOIN TB_PaymentTerm PT ON PT.Id = CC.TermOfPayment
                WHERE CC.Id = @CCId

            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<IEnumerable<EmailForAlertRequestApproveModel>> GetEmailForSendAlertApproveRequest()
    {
      return await _dbTransaction.Connection.QueryAsync<EmailForAlertRequestApproveModel>(@"
                SELECT 
                CT.Email
                FROM TB_CreditControlApproveTrans CT
                WHERE 
                CT.SendEmailDate IS NOT NULL
                AND CT.ApproveDate IS NULL
                AND CT.RejectDate IS NULL
                AND CT.IsDone = 0
                GROUP BY 
                CT.Email
            ", null, _dbTransaction);
    }

    public async Task<IEnumerable<CreditControlAttachFileTable>> GetFileByCCIdAsync(int ccId, string requestType)
    {
      return await _dbTransaction.Connection.QueryAsync<CreditControlAttachFileTable>($@"
                SELECT *
                FROM TB_CreditControlAttachFile CF 
                WHERE CF.CCId = {ccId}
                AND CF.CCType = '{requestType}'
            ", null, _dbTransaction);
    }

    public async Task<ViewOVSViewModel> GetOVSDataByCCIdAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ViewOVSViewModel>($@"

                Declare @val Varchar(MAX); 
                Select @val = COALESCE(@val + ', ' + CT.Description, CT.Description) 
                From TB_CustomerTypeByProductTrans  CTT 
                left join TB_CustomerType CT ON CT.Id = CTT.CustomerByProductId
                where CTT.CCId = @CCId;

                SELECT 
                CC.RequestNumber,
                CC.RequestDate,
                CC.CustomerCode,
                CT.Description AS TypeOfBusiness,
                @val AS TypeOfProduct,
                CC.CompanyName,
                CASE
                    WHEN CC.IsHeadQuarter = 1 THEN 'สาขาใหญ่'
                    ELSE 'สาขาย่อย'
                END AS IsHeadQuarter,
                CC.Branch,
                AD1.AddressNo,
                AD1.Street,
                AD1.ZipCode,
                AD1.City,
                AD1.Country,
                AD1.ContactPerson1,
                AD1.ContactPerson1_Tel,
                AD1.ContactPerson1_Email,
                AD1.ContactPerson2,
                AD1.ContactPerson2_Tel,
                AD1.ContactPerson2_Email,
                E.Name + ' ' + E.LastName AS SaleName,
                SR.DSG_SALESREGIONNAME AS SaleZone,
                DT.TXT AS DeliveryCondition,
                CRC.TXT AS Currency,
                CC.DestinationPort,
                CC.DimensionNo,
                CC.DiscountByConstant,
                CC.DiscountByStep,
                CC.TermOfDelivery,
                CC.TermOfDelivery_Other,
                CC.CreditLimited,
                CC.CreditRating,
                CASE 
                  WHEN CC.GuaranteeOVS_Check = 1 THEN 'StandBy LC Amount (USD)'
                  WHEN CC.GuaranteeOVS_Check = 2 THEN 'Security Deposit Amount'
                  ELSE 'Other'
                END AS GuaranteeOVS_Check,
                CC.GuaranteeOVS_ExpireDate,
                CC.GuaranteeOVS_IssueDate,
                CC.GuaranteeOVS_Other,
                CC.GuaranteeOVS_SecurityDepositAmount,
                CC.GuaranteeOVS_StandbyLCAmount,
                CC.CommentFromSale
                FROM TB_CreditControl CC
                LEFT JOIN TB_Address AD1 ON AD1.CCId = CC.Id AND AD1.AddressType = 'Address1_OVS'
                LEFT JOIN TB_Employee E ON E.EmployeeId = CC.SaleEmployeeId
                LEFT JOIN TB_SalesRegion SR ON SR.Id = CC.SaleZone
                LEFT JOIN TB_DeliveryTerm DT ON DT.Id = CC.DeliveryCondition
                LEFT JOIN TB_Currency CRC ON CRC.Id = CC.Currency
                LEFT JOIN TB_CustomerType CT ON CT.Id = CC.TypeOfBusiness
                WHERE CC.Id = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<IEnumerable<PaymentMethodTransTable>> GetPaymentMethodTransByCCIdAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryAsync<PaymentMethodTransTable>($@"
                SELECT *
                FROM TB_PaymentMethodTrans P
                WHERE P.CCId = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<RequesterInfoViewModel> GetRequesterInfoAsync(int creditControlId)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<RequesterInfoViewModel>($@"
                SELECT
                E.Name + ' ' + E.LastName AS Name
                FROM TB_CreditControl CC
                LEFT JOIN TB_Employee E ON E.EmployeeId = CC.SaleEmployeeId 
                WHERE CC.Id = @CCId
            ", new { @CCId = creditControlId }, _dbTransaction);
    }

    public async Task<IEnumerable<RequestWaitingToApproveModel>> GetRequestWaitingToApproveByEmail(string email)
    {
      return await _dbTransaction.Connection.QueryAsync<RequestWaitingToApproveModel>(@"
                SELECT 
	                X.Id AS TransId, 
	                X.CCId, 
	                X.Email, 
	                CC.RequestNumber, 
	                CC.CompanyName 
                FROM 
                (
	                SELECT 
	                CT.Id,
	                CT.CCId,
	                CT.Email
	                FROM TB_CreditControlApproveTrans CT
	                WHERE 
	                CT.SendEmailDate IS NOT NULL
	                AND CT.ApproveDate IS NULL
	                AND CT.RejectDate IS NULL
	                AND CT.IsDone = 0
	                GROUP BY 
	                CT.Email,
	                CT.Id,
	                CT.CCId
                ) X
                LEFT JOIN TB_CreditControl CC ON CC.Id = X.CCId
                WHERE X.Email = @Email
            ", new { @Email = email }, _dbTransaction);
    }
  }
}
