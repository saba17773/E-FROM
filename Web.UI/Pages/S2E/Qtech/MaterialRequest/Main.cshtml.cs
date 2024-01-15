using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech.MaterialRequest
{
    public class MainModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int isPurchase { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        public MainModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService,
         IEmailService emailService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
            _emailService = emailService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUEST));

                if (await _authService.CanDisplay(nameof(S2EPermissionModel.VIEW_PURCHASE)))
                {
                    isPurchase = 1;
                }
                else
                {
                    isPurchase = 0;
                }

                isPurchase = 0;
                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }

        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        REQUESTCODE = "REQUESTCODE",
                        REQUESTNO = "PROJECTREFNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        ITEMCODE = "ITEMCODE",
                        ITEMNAME = "ITEMNAME"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaterialRequestGridModel>(@"
                        SELECT *
                        FROM
                        (
	                        SELECT T2.*,
	                        CASE 
		                        WHEN (T2.SUMSUCCESSSTATUS = T2.COUNTTOTALREQLINE) AND (T2.ADDRMQTY = T2.RMREQQTYTOTAL)  
		                        THEN 1 
		                        ELSE 0 
	                        END AS ISCOMPLETE
	                        FROM
	                        (
		                        SELECT T1.ADDRMID,
		                        T1.REQUESTCODE,
		                        T1.VENDORID,
		                        T1.SUPPLIERNAME,
		                        T1.ITEMCODE,
		                        T1.ITEMNAME,
		                        T1.ADDRMAPPROVESTATUS,
		                        T1.ADDRMLINEID,
		                        T1.PROJECTREFNO,
		                        T1.RMREQID,
		                        T1.REQUESTSTATUS,
		                        SUM(T1.RMREQQTY)RMREQQTYTOTAL,
		                        T1.ADDRMQTY,
		                        SUM(REQAPPROVESTATUS)AS SUMSUCCESSSTATUS,
		                        COUNT(RMREQLINEID) AS COUNTTOTALREQLINE
		                        FROM (
			                        SELECT T.ADDRMID,R.REQUESTCODE,T.VENDORID,
				                           LEFT(T.SUPPLIERNAME, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
				                           PS.ITEMCODE,PS.ITEMNAME,
				                           AL.APPROVESTATUS ADDRMAPPROVESTATUS,
				                           AL.ID ADDRMLINEID,LL.PROJECTREFNO,
				                           MR.ID RMREQID,MR.REQUESTSTATUS,
				                           ML.ID RMREQLINEID,
				                           CASE WHEN ML.APPROVESTATUS = 7 THEN 1 ELSE 0 END AS REQAPPROVESTATUS,
				                           ML.QTY AS RMREQQTY,T.QTYALL ADDRMQTY
			                        FROM
			                        (
				                        SELECT AH.ID ADDRMID,AH.REQUESTID,AH.ASSESSMENTID,AH.LABID,AH.LABLINEID,
				                        AH.PCSAMPLEID,AH.PLANT,AH.ISACTIVE,SUM(AL.QTY) QTYALL,
				                        AH.VENDORID,AH.SUPPLIERNAME
				                        FROM TB_S2EAddRawMaterialHead AH JOIN
				                        TB_S2EAddRawMaterialLine AL ON AH.ID = AL.ADDRMID
				                        WHERE AL.APPROVESTATUS IN (5,7) AND AH.ISACTIVE = 1
				                        GROUP BY AH.ID,AH.REQUESTID,AH.ASSESSMENTID,AH.LABID,AH.LABLINEID,
				                        AH.PCSAMPLEID,AH.PLANT,AH.ISACTIVE,AH.VENDORID,AH.SUPPLIERNAME
			                        )T JOIN
			                        TB_S2EAddRawMaterialLine AL ON T.ADDRMID = AL.ADDRMID 
			                        AND AL.ISCURRENTLOGS = 1 JOIN
			                        TB_S2ENewRequest R ON T.REQUESTID = R.ID JOIN
			                        TB_S2EPurchaseSample PS ON T.PCSAMPLEID = PS.ID JOIN
			                        TB_S2ELABTestHead LH ON T.LABID = LH.ID LEFT JOIN
			                        TB_S2ELABTestLine LL ON T.LABLINEID = LL.ID LEFT JOIN
			                        TB_S2EMaterialRequestHead MR ON T.ADDRMID = MR.ADDRMID LEFT JOIN
			                        TB_S2EMaterialRequestLine ML ON MR.ID = ML.RMREQID AND ML.APPROVESTATUS <> 2
		                        )T1
		                        GROUP BY T1.ADDRMID,T1.REQUESTCODE,T1.VENDORID,T1.SUPPLIERNAME,
		                        T1.ITEMCODE,T1.ITEMNAME,T1.ADDRMAPPROVESTATUS,T1.ADDRMLINEID,
		                        T1.PROJECTREFNO,T1.RMREQID,T1.REQUESTSTATUS,T1.ADDRMQTY
	                        )T2
                        )T3
                        WHERE " + filter + @" ORDER BY T3.REQUESTSTATUS
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnGetRequestMoreRMAsync(int RMREQID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                    MaterialReqHeadByID.REQUESTSTATUS = RequestStatusModel.Open;

                    var ADDRMID = MaterialReqHeadByID.ADDRMID;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    AddRMHeadByID.ISADDMORE = 1;

                    var RequestID = AddRMHeadByID.REQUESTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var CreateBy = _authService.GetClaim().UserId;
                    var CreateDate = DateTime.Now;

                    //UPDATE OLD LOGS SENDMAIL
                    var LogsSendmailRepo = new GenericRepository<S2EAddMoreRawMaterialLogsSendEmail_TB>(unitOfWork.Transaction);
                    var LogsSendmailALL = await LogsSendmailRepo.GetAllAsync();
                    var LogsSendmailOLD = LogsSendmailALL.Where(x=>x.RMREQID == RMREQID && x.ISLASTSENDEMAIL == 1);
                    if (LogsSendmailOLD.ToList().Count != 0)
                    {
                        foreach (var Logs in LogsSendmailOLD)
                        {
                            var SendEmailOldUpdate = await LogsSendmailRepo.GetAsync(Logs.ID);
                            SendEmailOldUpdate.ISLASTSENDEMAIL = 0;
                            await LogsSendmailRepo.UpdateAsync(SendEmailOldUpdate);
                        }
                    }



                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISADDMORERM == 1
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;

                    //GET ApproveFlow
                    var AppFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                    var AppFlowALL = await AppFlowRepo.GetAllAsync();
                    var AppFlowByAppMasterID = AppFlowALL.Where(x => x.ApproveMasterId == approvemasterid & x.IsActive == 1);

                    // INSERT Logs SendMail
                    foreach (var AppFlow in AppFlowByAppMasterID)
                    {
                        await LogsSendmailRepo.InsertAsync(new S2EAddMoreRawMaterialLogsSendEmail_TB
                        {
                            RMREQID = RMREQID,
                            EMAIL = AppFlow.Email,
                            SENDEMAILBY = CreateBy,
                            SENDEMAILDATE = CreateDate,
                            APPROVEGROUPID = ApproveGroupID,
                            APPROVEMASTERID = approvemasterid,
                            ISLASTSENDEMAIL = 1
                        });

                        var BodyEmail = $@"{RequestCode} ต้องการขอซื้อวัตถุดิบเพื่อทดสอบเพิ่มเติม";


                        var sendEmail = _emailService.SendEmail(
                                  $"{RequestCode} / ต้องการขอซื้อวัตถุดิบเพื่อทดสอบเพิ่มเติม",
                                  BodyEmail,
                                  new List<string> { AppFlow.Email },
                                  new List<string> { },
                                  "",
                                  "",
                                  new List<string> { }
                            );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }

                    }

                    await AddRMHeadRepo.UpdateAsync(AddRMHeadByID);
                    await MaterialReqHeadRepo.UpdateAsync(MaterialReqHeadByID);

                    unitOfWork.Complete();

                    AlertSuccess = "แจ้งขอเพิ่มวัตถุดิบเข้าระบบเพิ่มเรียบร้อย";
                    return Redirect("/S2E/Qtech/MaterialRequest/Main");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
