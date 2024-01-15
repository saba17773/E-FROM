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

namespace Web.UI.Pages.S2E.Qtech.MaterialRequest
{
    public class RequestDetailModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public int RMReqLineID { get; set; }
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
        public async Task<IActionResult> OnGetAsync(int RMREQID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqID = RMREQID;
                    await GetData(RMREQID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }
        }
        public async Task GetData(int RMREQID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);
                
                var AddRMID = MaterialReqHeadByID.ADDRMID;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x=>x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1 && (x.APPROVESTATUS == 5 || x.APPROVESTATUS == 7)).FirstOrDefault();

                if (AddRMLineByID == null)
                {
                    QtyTotal = 0;
                }
                else
                {
                    var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                    var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID &&
                                                                                x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                                x.ISACTIVE == 1 &&
                                                                                x.APPROVESTATUS != 2);
                    decimal QtyUse = 0;
                    if (MaterialReqLineByReqID != null)
                    {
                        foreach (var MaterialReqLineQTY in MaterialReqLineByReqID)
                        {
                            QtyUse += MaterialReqLineQTY.QTY;
                        }
                    }

                    QtyTotal = AddRMLineByID.QTY - QtyUse;
                }
               

                var RequestID = AddRMHeadByID.REQUESTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);


                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = MaterialReqHeadByID.REQUESTSTATUS;
                Plant = MaterialReqHeadByID.PLANT;
                ItemGroup = MaterialReqHeadByID.ITEMGROUP;
                ItemCode = MaterialReqHeadByID.ITEMCODE;
                ItemName = MaterialReqHeadByID.ITEMNAME;
                Unit = MaterialReqHeadByID.UNIT;

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostGridAsync(int RMREQID)
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
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaterialRequestDetailGridModel>(@"
                        SELECT *
                          FROM
                          (
	                          SELECT RH.ID RMREQID,
		                             RH.ADDRMID,
		                             RL.ID RMREQLINEID,
		                             RL.NO,
		                             CONVERT(VARCHAR,RL.REQUESTDATE,103) + ' ' + CONVERT(VARCHAR,RL.REQUESTDATE,108) REQUESTDATE,
		                             RL.DEPARTMENT,
		                             RL.SUPGROUP,
		                             RL.QTY,
		                             RH.UNIT,
		                             RL.APPROVEMASTERID,
		                             RL.APPROVESTATUS,
		                             RL.APPROVEGROUPID,
		                             AG.GROUPDESCRIPTION,
		                             U.Username REQUESTBY
	                          FROM TB_S2EMaterialRequestHead RH JOIN
	                          TB_S2EMaterialRequestLine RL ON RL.RMREQID = RH.ID JOIN
	                          TB_S2EApproveGroup AG ON RL.APPROVEGROUPID = AG.ID JOIN
	                          TB_User U ON RL.CREATEBY = U.Id
                          )T
                          WHERE T.RMREQID = @RMREQID AND APPROVESTATUS <> 2
                        AND " + filter + @" ORDER BY T.RMREQLINEID
                    ", 
                    new
                    {
                        @RMREQID = RMREQID
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
        public async Task<IActionResult> OnPostGridApproveTransAsync(int RMREQID, int RMREQLineID)
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
                                    ,AT.ISDONE,AT.REMARK,AT.RMREQID,AT.RMREQLINEID,AG.GROUPDESCRIPTION
                                FROM TB_S2EMaterialRequestApproveTrans AT JOIN
                                TB_S2EApproveGroup AG ON AT.APPROVEGROUPID = AG.ID
                                WHERE  AT.RMREQID = {RMREQID} AND AT.RMREQLINEID = {RMREQLineID}
                            )T
                            GROUP BY T.ID,APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                                T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,T.GROUPDESCRIPTION,
                                T.APPROVEGROUPID,T.RMREQID,T.RMREQLINEID
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
                    var MaterialReqAppRepo = new GenericRepository<S2EMaterialRequestApproveTrans_TB>(unitOfWork.Transaction);
                    var MaterialReqAppByID = await MaterialReqAppRepo.GetAsync(TranID);
                    var RMReqID = MaterialReqAppByID.RMREQID;
                    var RMReqLineID = MaterialReqAppByID.RMREQLINEID;
                    var ApproveLevel = MaterialReqAppByID.APPROVELEVEL;

                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMReqID);

                    var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMReqLineID);

                    var AddRMID = MaterialReqHeadByID.ADDRMID;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var CreateBy = MaterialReqLineByID.CREATEBY;
                    var ApproveMasterid = MaterialReqLineByID.APPROVEMASTERID;
                    var RequestDate = MaterialReqLineByID.REQUESTDATE;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var appoveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                    var ApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == ApproveMasterid &&
                                                              x.ApproveLevel == ApproveLevel &&
                                                              x.IsActive == 1).Select(s => s.Name).FirstOrDefault();

                    var PlantBodyEmail = "";
                    if (MaterialReqHeadByID.PLANT == "DSL")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน จำกัด";
                    }
                    else if (MaterialReqHeadByID.PLANT == "DRB")
                    {
                        PlantBodyEmail = "บริษัท ดีรับเบอร์ จำกัด";
                    }
                    else if (MaterialReqHeadByID.PLANT == "DSI")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน อินเตอร์เนชั่นแนล จำกัด";
                    }
                    else if (MaterialReqHeadByID.PLANT == "DSR")
                    {
                        PlantBodyEmail = "บริษัท สวิซซ์-วัน คอร์ปอเรชั่น จำกัด";
                    }
                    else
                    {
                        PlantBodyEmail = "บริษัท สยามทรัค เรเดียล จำกัด";
                    }

                    var ischeck1 = "";
                    var ischeck2 = "";
                    if (MaterialReqHeadByID.ITEMGROUP == "RM")
                    {
                        ischeck1 = "checked";
                        ischeck2 = "";
                    }
                    else
                    {
                        ischeck1 = "";
                        ischeck2 = "checked";
                    }
                    var No = MaterialReqLineByID.NO;
                    var BodyEmail = "";
                    BodyEmail = $@"
                        <b> {PlantBodyEmail} </b> <br/>
                        <b> ใบเบิกวัตถุดิบ </b> <br/>
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
                                    {MaterialReqLineByID.NO}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                     วันที่ : 
                                </td>
                                <td style='text-align:left;'>
                                    {Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                     แผนก : 
                                </td>
                                <td style='text-align:left;'>
                                    {MaterialReqLineByID.DEPARTMENT}
                                </td>
                            </tr>
                            <tr>
                                <tr>
                                <td style='text-align:right;'>
                                     ฝ่าย : 
                                </td>
                                <td style='text-align:left;'>
                                    {MaterialReqLineByID.SUPGROUP}
                                </td>
                            </tr>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                     Item Group : 
                                </td>
                                <td style='text-align:left;'>
                                    <label>
                                        <input type = 'radio' id = 'RM' name = 'ItemGroup' {ischeck1} disabled>
                                        RM
                                    </label>
                                    <label>
                                        <input type = 'radio' id = 'FS' name = 'ItemGroup' {ischeck2} disabled>
                                        FS
                                    </label>
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    Item Code :
                                </td>
                                <td style='text-align:left;'>
                                    {MaterialReqHeadByID.ITEMCODE}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    Item Name :
                                </td>
                                <td style='text-align:left;'>
                                    {MaterialReqHeadByID.ITEMNAME}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    จำนวน :
                                </td>
                                <td style='text-align:left;'>
                                    {String.Format("{0:#,##0.#0}", MaterialReqLineByID.QTY)}
                                </td>
                            </tr>
                            <tr>
                                <td style='text-align:right;'>
                                    หน่วย :
                                </td>
                                <td style='text-align:left;'>
                                    {MaterialReqHeadByID.UNIT}
                                </td>
                            </tr>
                        </table>
                        <br/>
                        <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/MaterialRequestTodolist?Email={MaterialReqAppByID.EMAIL}'> คลิกที่นี่ </a>
                        <br/>
                    ";

                    var sendEmail = _emailService.SendEmail(
                        $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ / No.: {No}",
                        BodyEmail,
                        new List<string> { MaterialReqAppByID.EMAIL },
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
                    return Redirect($"/S2E/Qtech/MaterialRequest/{RMReqID}/RequestDetail");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/MaterialRequest/Main");
            }
        }
    }
}
