using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech.MaterialRequest
{
    public class CreateModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public int No { get; set; }
        [BindProperty]
        public string Department { get; set; }
        [BindProperty]
        public string SupGroup { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int AddRMID { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public int RMReqLineID { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public CreateModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int ADDRMID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var MaterialREQHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialREQHeadALL = await MaterialREQHeadRepo.GetAllAsync();
                    var CheckRequest = MaterialREQHeadALL.Where(x => x.ADDRMID == ADDRMID).FirstOrDefault();
                    if (CheckRequest != null)
                    {
                        AlertError = "Request นี้ได้ทำการสร้างแล้ว";
                        return Redirect($"/S2E/Qtech/MaterialRequest/Main");
                    }

                    AddRMID = ADDRMID;
                    await GetData(ADDRMID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/MaterialRequest/Main");
            }
        }
        public async Task GetData(int ADDRMID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLine = AddRMLineALL.Where(x=>x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                var RequestID = AddRMHeadByID.REQUESTID;
                var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                var LABID = AddRMHeadByID.LABID;
                var LABLINEID = AddRMHeadByID.LABLINEID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                RequestCode = NewRequestByID.REQUESTCODE;
                Qty = AddRMLineByID.QTY;
                Plant = AddRMHeadByID.PLANT;
                ItemGroup = LABTestHeadByID.ITEMGROUP;
                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;
                Unit = NewRequestByID.UNIT;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int ADDRMID,string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                AddRMID = ADDRMID;
                await GetData(ADDRMID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RequestDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;
                
                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var LABID = AddRMHeadByID.LABID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadALL = await MaterialReqHeadRepo.GetAllAsync();

                    var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    //DSL000001
                    //GENERATE NO.
                    var PlantGen = AddRMHeadByID.PLANT;
                    var No = "";
                    var maxNo = MaterialReqLineALL.Where(x=>x.NO.Substring(0, 3) == PlantGen)
                                                 .Max(a => a.NO.Substring(3, 6));
                    if (maxNo == null)
                    {
                        No = PlantGen + "000001";
                    }
                    else
                    {
                        int NoAutorun = Int32.Parse(maxNo) + 1;
                        No = PlantGen + NoAutorun.ToString().PadLeft(6,'0');
                    }

                    if (Qty >  AddRMLineByID.QTY )
                    {
                        AlertError = "จำนวนของใน Store ไม่พอ";
                        return Redirect($"/S2E/Qtech/MaterialRequest/{ADDRMID}/Create");
                    }

                    //UPDATE Add Raw Material
                    AddRMLineByID.APPROVESTATUS = RequestStatusModel.Successfully;
                    AddRMLineByID.COMPLETEBY = CreateBy;
                    AddRMLineByID.COMPLETEDATE = RequestDate;
                    await AddRMLineRepo.UpdateAsync(AddRMLineByID);

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISREQUESTRM == 1
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;

                    //Draft or Save
                    var ApproveStatus = 0;
                    if (!string.IsNullOrEmpty(draft))
                    {
                        ApproveStatus = RequestStatusModel.Draft;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        ApproveStatus = RequestStatusModel.WaitingForApprove;
                    }
                    var QtyTotal = AddRMLineByID.QTY;
                    var RequestStatus = 0;
                    if (QtyTotal == Qty)
                    {
                        RequestStatus = RequestStatusModel.Complete;
                    }
                    else
                    {
                        RequestStatus = RequestStatusModel.Open;
                    }
                    //INSERT MATERIAL REQUEST HEAD
                    var MaterialReqHeadInsert = new S2EMaterialRequestHead_TB {
                        ADDRMID = ADDRMID,
                        PLANT = AddRMHeadByID.PLANT,
                        ITEMGROUP = LABTestHeadByID.ITEMGROUP,
                        ITEMCODE = PCSampleByID.ITEMCODE,
                        ITEMNAME = PCSampleByID.ITEMNAME,
                        UNIT = NewRequestByID.UNIT,
                        REQUESTSTATUS = RequestStatus
                    };
                    var RMREQID = await MaterialReqHeadRepo.InsertAsync(MaterialReqHeadInsert);

                    //INSERT MATERIAL REQUEST LINE
                    var MaterialReqLineInsert = new S2EMaterialRequestLine_TB
                    {
                        ADDRMLINEID = AddRMLineByID.ID,
                        RMREQID = (int)RMREQID,
                        REQUESTDATE = RequestDate,
                        NO = No,
                        QTY = Qty,
                        DEPARTMENT = Department,
                        SUPGROUP = SupGroup,
                        APPROVEMASTERID = approvemasterid,
                        APPROVEGROUPID = ApproveGroupID,
                        CURRENTAPPROVESTEP = 1,
                        APPROVESTATUS = ApproveStatus,
                        ISACTIVE = 1,
                        CREATEBY = CreateBy,
                        CREATEDATE = RequestDate
                    };
                    var RMREQLineID = await MaterialReqLineRepo.InsertAsync(MaterialReqLineInsert);

                    //Save 
                    if (!string.IsNullOrEmpty(save))
                    {
                        //GET APPROVE FLOW BY APPROVE MASTER ID
                        var appoveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                        var approveFlow_data = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel != 0 &&
                                                                        x.IsActive == 1
                                                                   ).OrderBy(o => o.ApproveLevel);

                        // GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        var nonceRepo = new GenericRepository<S2EMaterialRequestNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2EMaterialRequestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = RequestDate,
                            EXPIREDATE = RequestDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT APPROVE TRANSECTION
                        var MaterialReqAppTranRepo = new GenericRepository<S2EMaterialRequestApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {

                            await MaterialReqAppTranRepo.InsertAsync(new S2EMaterialRequestApproveTrans_TB
                            {
                                RMREQID = (int)RMREQID,
                                RMREQLINEID = (int)RMREQLineID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1
                            });
                        }

                        var currentRecord = await MaterialReqLineRepo.GetAsync((int)RMREQLineID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await MaterialReqLineRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByMaterialReqID((int)RMREQID, (int)RMREQLineID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await MaterialReqAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = RequestDate;
                            await MaterialReqAppTranRepo.UpdateAsync(AppTransByID);

                            var PlantBodyEmail = "";
                            if (AddRMHeadByID.PLANT == "DSL")
                            {
                                PlantBodyEmail = "บริษัท ดีสโตน จำกัด";
                            }
                            else if (AddRMHeadByID.PLANT == "DRB")
                            {
                                PlantBodyEmail = "บริษัท ดีรับเบอร์ จำกัด";
                            }
                            else if (AddRMHeadByID.PLANT == "DSI")
                            {
                                PlantBodyEmail = "บริษัท ดีสโตน อินเตอร์เนชั่นแนล จำกัด";
                            }
                            else if (AddRMHeadByID.PLANT == "DSR")
                            {
                                PlantBodyEmail = "บริษัท สวิซซ์-วัน คอร์ปอเรชั่น จำกัด";
                            }
                            else
                            {
                                PlantBodyEmail = "บริษัท สยามทรัค เรเดียล จำกัด";
                            }

                            var ischeck1 = "";
                            var ischeck2 = "";
                            if (LABTestHeadByID.ITEMGROUP == "RM")
                            {
                                ischeck1 = "checked";
                                ischeck2 = "";
                            }
                            else
                            {
                                ischeck1 = "";
                                ischeck2 = "checked";
                            }

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
                                            {No}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             วันที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RequestDate.ToString("dd/MM/yyyy HH:mm:ss")}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             แผนก : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {Department}
                                        </td>
                                    </tr>
                                    <tr>
                                        <tr>
                                        <td style='text-align:right;'>
                                             ฝ่าย : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {SupGroup}
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
                                            {PCSampleByID.ITEMCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Name :
                                        </td>
                                        <td style='text-align:left;'>
                                            {PCSampleByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            จำนวน :
                                        </td>
                                        <td style='text-align:left;'>
                                            {String.Format("{0:#,##0.#0}", Qty)}
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
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/MaterialRequestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                <br/>
                            ";

                            var sendEmail = _emailService.SendEmail(
                                   $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ / No.: {No}",
                                   BodyEmail,
                                   new List<string> { AppTrans.EMAIL },
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

                    }

                    unitOfWork.Complete();
                    AlertSuccess = "สร้างใบเบิกวัตถุดิบสำเร็จ";
                    return Redirect($"/S2E/Qtech/MaterialRequest/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Qtech/MaterialRequest/{ADDRMID}/Create");
            }
        }
    }
}
