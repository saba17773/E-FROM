using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech.MaterialRequestSample
{
    public class RequestDetailModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int RMReqSamID { get; set; }
        [BindProperty]
        public int RMReqSamLineID { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int RequestStatus { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public RequestDetailModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService,
         IEmailService emailService,
         IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int RMREQSAMID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUESTSAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqSamID = RMREQSAMID;
                    await GetData(RMREQSAMID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }
        }
        public async Task GetData(int RMREQSAMID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMREQSAMID);

                var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RequestID = AddRMSampleByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);


                var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                var RMReqSampleLineALL = await RMReqSampleLineRepo.GetAllAsync();
                var RMReqSampleLineByReqID = RMReqSampleLineALL.Where(x => x.RMREQSAMID == RMREQSAMID &&
                                                                            x.ISACTIVE == 1 &&
                                                                            x.APPROVESTATUS != 2);
                decimal QtyUse = 0;
                if (RMReqSampleLineByReqID != null)
                {
                    foreach (var RMReqSampleLineQTY in RMReqSampleLineByReqID)
                    {
                        QtyUse += RMReqSampleLineQTY.QTY;
                    }
                }

                QtyTotal = NewRequestByID.QTY - QtyUse;

                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = RMReqSampleHeadByID.REQUESTSTATUS;
                Plant = AddRMSampleByID.PLANT;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Unit = NewRequestByID.UNIT;

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostGridAsync(int RMReqSamID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        NO = "NO",
                        REQUESTDATE = "REQUESTDATE",
                        DEPARTMENT = "DEPARTMENT",
                        SUPGROUP = "SUPGROUP",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaterialRequestSampleDetailGridModel>(@"
                        SELECT *
                        FROM
                        (
	                        SELECT H.ID RMREQSAMID,
		                        H.ADDRMSAMPLEID,
		                        L.ID RMREQSAMLINEID,
		                        L.NO,
		                        CONVERT(VARCHAR,L.REQUESTDATE,103) + ' ' + CONVERT(VARCHAR,L.REQUESTDATE,108) REQUESTDATE,
		                        L.DEPARTMENT,
		                        L.SUPGROUP,
		                        L.QTY,
		                        R.UNIT,
		                        L.APPROVEMASTERID,
		                        L.APPROVESTATUS,
		                        L.APPROVEGROUPID,
		                        G.GROUPDESCRIPTION,
		                        U.Username REQUESTBY
	                        FROM TB_S2EMaterialRequestSampleHead H JOIN
	                        TB_S2EMaterialRequestSampleLine L ON L.RMREQSAMID = H.ID JOIN
	                        TB_S2EAddRawMaterialSample S ON H.ADDRMSAMPLEID = S.ID JOIN
	                        TB_S2ENewRequest R ON S.REQUESTID = R.ID JOIN
	                        TB_S2EApproveGroup G ON L.APPROVEGROUPID = G.ID JOIN
	                        TB_User U ON L.CREATEBY = U.Id
                        )T
                        WHERE T.RMREQSAMID = @RMREQSAMID AND APPROVESTATUS <> 2
                        AND " + filter + @" ORDER BY T.RMREQSAMLINEID
                    ",
                    new
                    {
                        @RMREQSAMID = RMReqSamID
                    }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostGridApproveTransAsync(int RMReqSamID, int RMReqSamLineID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EApproveTransGridModel>($@"
                           SELECT *
                            FROM
                            (
                                SELECT AT.ID,AT.APPROVEGROUPID,AT.APPROVEMASTERID,AT.EMAIL,AT.APPROVELEVEL
                                    ,CONVERT(VARCHAR,AT.SENDEMAILDATE,103)+' '+CONVERT(VARCHAR,AT.SENDEMAILDATE,108)SENDEMAILDATE
                                    ,CONVERT(VARCHAR,AT.APPROVEDATE,103)+' '+CONVERT(VARCHAR,AT.APPROVEDATE,108)APPROVEDATE
                                    ,CONVERT(VARCHAR,AT.REJECTDATE,103)+' '+CONVERT(VARCHAR,AT.REJECTDATE,108)REJECTDATE
                                    ,AT.ISDONE,AT.REMARK,AT.RMREQSAMID,AT.RMREQSAMLINEID,AG.GROUPDESCRIPTION
                                FROM TB_S2EMaterialRequestSampleApproveTrans AT JOIN
                                TB_S2EApproveGroup AG ON AT.APPROVEGROUPID = AG.ID
                                WHERE  AT.RMREQSAMID = {RMReqSamID} AND AT.RMREQSAMLINEID = {RMReqSamLineID}
                            )T
                            GROUP BY T.ID,APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                                T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,T.GROUPDESCRIPTION,
                                T.APPROVEGROUPID,T.RMREQSAMID,T.RMREQSAMLINEID
                            ORDER BY T.ID ASC
                    ", null, unitOfWork.Transaction);
                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnGetResendEmailAsync(int TranID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RMReqSampleAppRepo = new GenericRepository<S2EMaterialRequestSampleApproveTrans_TB>(unitOfWork.Transaction);
                    var RMReqSampleAppByID = await RMReqSampleAppRepo.GetAsync(TranID);
                    var RMReqSamID = RMReqSampleAppByID.RMREQSAMID;
                    var RMReqSamLineID = RMReqSampleAppByID.RMREQSAMLINEID;
                    var ApproveLevel = RMReqSampleAppByID.APPROVELEVEL;

                    var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                    var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMReqSamID);

                    var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                    var RMReqSampleLineByID = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);

                    var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var CreateBy = RMReqSampleLineByID.CREATEBY;
                    var ApproveMasterid = RMReqSampleLineByID.APPROVEMASTERID;
                    var RequestDate = RMReqSampleLineByID.REQUESTDATE;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var appoveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                    var ApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == ApproveMasterid &&
                                                              x.ApproveLevel == ApproveLevel &&
                                                              x.IsActive == 1).Select(s => s.Name).FirstOrDefault();

                    var PlantBodyEmail = "";
                    if (AddRMSampleByID.PLANT == "DSL")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน จำกัด";
                    }
                    else if (AddRMSampleByID.PLANT == "DRB")
                    {
                        PlantBodyEmail = "บริษัท ดีรับเบอร์ จำกัด";
                    }
                    else if (AddRMSampleByID.PLANT == "DSI")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน อินเตอร์เนชั่นแนล จำกัด";
                    }
                    else if (AddRMSampleByID.PLANT == "DSR")
                    {
                        PlantBodyEmail = "บริษัท สวิซซ์-วัน คอร์ปอเรชั่น จำกัด";
                    }
                    else
                    {
                        PlantBodyEmail = "บริษัท สยามทรัค เรเดียล จำกัด";
                    }

                    var No = RMReqSampleLineByID.NO;
                    var BodyEmail = "";
                    BodyEmail = $@"
                        <b> {PlantBodyEmail} </b> <br/>
                        <b> ใบเบิกวัตถุดิบ (LAB Sample)</b> <br/>
                        <table>
                            <tr>
                                <td style='text-align:right;'>
                                     Request Code : 
                                </td>
                                <td style='text-align:left;'>
                                    {NewRequestByID.REQUESTCODE}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                     เลขที่ : 
                                </td>
                                <td style='text-align:left;'>
                                    {RMReqSampleLineByID.NO}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                     วันที่ : 
                                </td>
                                <td style='text-align:left;'>
                                    {Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                     แผนก : 
                                </td>
                                <td style='text-align:left;'>
                                    {RMReqSampleLineByID.DEPARTMENT}
                                </td>
                            </tr>
                            <tr>
                                <tr>
                                <td style='text-align:right;'>
                                     ฝ่าย : 
                                </td>
                                <td style='text-align:left;'>
                                    {RMReqSampleLineByID.SUPGROUP}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    Item Name :
                                </td>
                                <td style='text-align:left;'>
                                    {NewRequestByID.ITEMNAME}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    จำนวน :
                                </td>
                                <td style='text-align:left;'>
                                    {String.Format("{0:#,##0.#0}", RMReqSampleLineByID.QTY)}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    หน่วย :
                                </td>
                                <td style='text-align:left;'>
                                    {NewRequestByID.UNIT}
                                </td>
                            </tr>
                        </table>
                        <br/>
                        <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/MaterialRequestSampleTodolist?Email={RMReqSampleAppByID.EMAIL}'> คลิกที่นี่ </a>
                        <br/>
                    ";

                    var sendEmail = _emailService.SendEmail(
                        $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ (LAB Sample) / No.: {No}",
                        BodyEmail,
                        new List<string> { RMReqSampleAppByID.EMAIL },
                        new List<string> { },
                        "",
                        "",
                        new List<string> { }
                    );

                    if (sendEmail.Result == false)
                    {
                        return new JsonResult(false);
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Resend Email Successfully No: " + No;
                    return Redirect($"/S2E/Qtech/MaterialRequestSample/{RMReqSamID}/RequestDetail");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/MaterialRequestSample/Main");
            }
        }
    }
}
