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
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string No { get; set; }
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
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public int RequestStatus { get; set; }
        [BindProperty]
        public string RequestDate { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public EditModel(
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
        public async Task<IActionResult> OnGetAsync(int RMREQLineID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqLineID = RMREQLineID;
                    await GetData(RMREQLineID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/MaterialRequest/Main");
            }
        }
        public async Task GetData(int RMREQLineID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMREQLineID);
                RMReqID = MaterialReqLineByID.RMREQID;
                var RMREQID = MaterialReqLineByID.RMREQID;

                var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                var ADDRMID = MaterialReqHeadByID.ADDRMID;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID && 
                                                                           x.ADDRMLINEID == AddRMLineByID.ID && 
                                                                           x.ISACTIVE == 1 &&
                                                                           x.ID != RMREQLineID &&
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

                No = MaterialReqLineByID.NO;
                Department = MaterialReqLineByID.DEPARTMENT;
                SupGroup = MaterialReqLineByID.SUPGROUP;
                Qty = MaterialReqLineByID.QTY;
                RequestDate = Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int RMREQLineID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                RMReqLineID = RMREQLineID;
                await GetData(RMREQLineID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RequestDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMREQLineID);
                    RMReqID = MaterialReqLineByID.RMREQID;
                    var RMREQID = MaterialReqLineByID.RMREQID;

                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);
                    var ADDRMID = MaterialReqHeadByID.ADDRMID;
                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var LABID = AddRMHeadByID.LABID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);
                    

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                    var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID &&
                                                                               x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                               x.ISACTIVE == 1 &&
                                                                               x.ID != RMREQLineID &&
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

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISREQUESTRM == 1
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;

                    if (Qty > QtyTotal)
                    {
                        AlertError = "จำนวนของใน Store ไม่พอ";
                        return Redirect($"/S2E/Qtech/MaterialRequest/{RMREQLineID}/Edit");
                    }

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

                    var RequestStatus = 0;
                    if (QtyTotal == Qty)
                    {
                        //UPDATE MATERIAL REQUEST HEAD
                        RequestStatus = RequestStatusModel.Complete;
                    }
                    else
                    {
                        RequestStatus = RequestStatusModel.Open;
                    }

                    MaterialReqHeadByID.REQUESTSTATUS = RequestStatus;
                    await MaterialReqHeadRepo.UpdateAsync(MaterialReqHeadByID);

                    MaterialReqLineByID.REQUESTDATE = RequestDate;
                    MaterialReqLineByID.DEPARTMENT = Department;
                    MaterialReqLineByID.SUPGROUP = SupGroup;
                    MaterialReqLineByID.QTY = Qty;
                    MaterialReqLineByID.APPROVESTATUS = ApproveStatus;
                    MaterialReqLineByID.UPDATEBY = CreateBy;
                    MaterialReqLineByID.UPDATEDATE = RequestDate;

                    await MaterialReqLineRepo.UpdateAsync(MaterialReqLineByID);

                    //Save 
                    if (!string.IsNullOrEmpty(save))
                    {
                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2EMaterialRequestApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.RMREQID == RMREQID && 
                                                                            x.RMREQLINEID == RMREQLineID && 
                                                                            x.APPROVEGROUPID == ApproveGroupID);
                        if (ApproveTransOld.ToList().Count != 0)
                        {
                            foreach (var App in ApproveTransOld)
                            {
                                var AppTransOldUpdate = await ApproveTransOldRepo.GetAsync(App.ID);
                                AppTransOldUpdate.ISCURRENTAPPROVE = 0;
                                await ApproveTransOldRepo.UpdateAsync(AppTransOldUpdate);
                            }
                        }

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
                                            {Qty}
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
                                   $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ / No.: {MaterialReqLineByID.NO} ",
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
                    return Redirect($"/S2E/Qtech/MaterialRequest/{RMREQID}/RequestDetail");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Qtech/MaterialRequest/{RMREQLineID}/Edit");
            }
        }
    }
}
