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

namespace Web.UI.Pages.S2E.Qtech.MaterialRequestSample
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
        public int AddRMSampleId { get; set; }
        [BindProperty]
        public int RMReqSamId { get; set; }
        [BindProperty]
        public int RMReqSamLineId { get; set; }
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
        public async Task<IActionResult> OnGetAsync(int RMReqSamLineID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RAWMATERIALREQUESTSAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqSamLineId = RMReqSamLineID;
                    await GetData(RMReqSamLineID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/MaterialRequestSample/Main");
            }
        }
        public async Task GetData(int RMReqSamLineID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                var RMReqSampleLineByID = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);
                var RMReqSamID = RMReqSampleLineByID.RMREQSAMID;

                RMReqSamId = RMReqSamID;

                var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMReqSamID);

                var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RequestID = AddRMSampleByID.REQUESTID;
                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                var RMReqSampleLineALL = await RMReqSampleLineRepo.GetAllAsync();
                var RMReqSampleLineByReqID = RMReqSampleLineALL.Where(x => x.RMREQSAMID == RMReqSamID &&
                                                                           x.ISACTIVE == 1 &&
                                                                           x.ID != RMReqSamLineID &&
                                                                           x.APPROVESTATUS != 2);
                decimal QtyUse = 0;
                if (RMReqSampleLineByReqID != null)
                {
                    foreach (var MaterialReqLineQTY in RMReqSampleLineByReqID)
                    {
                        QtyUse += MaterialReqLineQTY.QTY;
                    }
                }

                QtyTotal = NewRequestByID.QTY - QtyUse;

                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = RMReqSampleHeadByID.REQUESTSTATUS;
                Plant = AddRMSampleByID.PLANT;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Unit = NewRequestByID.UNIT;

                No = RMReqSampleLineByID.NO;
                Department = RMReqSampleLineByID.DEPARTMENT;
                SupGroup = RMReqSampleLineByID.SUPGROUP;
                Qty = RMReqSampleLineByID.QTY;
                RequestDate = Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int RMReqSamLineID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                RMReqSamLineId = RMReqSamLineID;
                await GetData(RMReqSamLineID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RequestDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                    var RMReqSampleLineByID = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);
                    var RMReqSamID = RMReqSampleLineByID.RMREQSAMID;

                    RMReqSamId = RMReqSamID;

                    var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                    var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMReqSamID);

                    var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RMReqSampleLineALL = await RMReqSampleLineRepo.GetAllAsync();
                    var RMReqSampleLineByReqID = RMReqSampleLineALL.Where(x => x.RMREQSAMID == RMReqSamID &&
                                                                               x.ISACTIVE == 1 &&
                                                                               x.ID != RMReqSamLineID &&
                                                                               x.APPROVESTATUS != 2);
                    decimal QtyUse = 0;
                    if (RMReqSampleLineByReqID != null)
                    {
                        foreach (var MaterialReqLineQTY in RMReqSampleLineByReqID)
                        {
                            QtyUse += MaterialReqLineQTY.QTY;
                        }
                    }

                    QtyTotal = NewRequestByID.QTY - QtyUse;

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISREQUESTRMSAMPLE == 1
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;

                    if (Qty > QtyTotal)
                    {
                        AlertError = "จำนวนของใน Store ไม่พอ";
                        return Redirect($"/S2E/Qtech/MaterialRequestSample/{RMReqSamLineID}/Edit");
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

                    RMReqSampleHeadByID.REQUESTSTATUS = RequestStatus;
                    await RMReqSampleHeadRepo.UpdateAsync(RMReqSampleHeadByID);

                    RMReqSampleLineByID.REQUESTDATE = RequestDate;
                    RMReqSampleLineByID.DEPARTMENT = Department;
                    RMReqSampleLineByID.SUPGROUP = SupGroup;
                    RMReqSampleLineByID.QTY = Qty;
                    RMReqSampleLineByID.APPROVESTATUS = ApproveStatus;
                    RMReqSampleLineByID.UPDATEBY = CreateBy;
                    RMReqSampleLineByID.UPDATEDATE = RequestDate;

                    await RMReqSampleLineRepo.UpdateAsync(RMReqSampleLineByID);

                    //Save 
                    if (!string.IsNullOrEmpty(save))
                    {
                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2EMaterialRequestSampleApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.RMREQSAMID == RMReqSamID &&
                                                                            x.RMREQSAMLINEID == RMReqSamLineID &&
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
                        var nonceRepo = new GenericRepository<S2EMaterialRequestSampleNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2EMaterialRequestSampleNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = RequestDate,
                            EXPIREDATE = RequestDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT APPROVE TRANSECTION
                        var RMReqSampleAppTranRepo = new GenericRepository<S2EMaterialRequestSampleApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {
                            await RMReqSampleAppTranRepo.InsertAsync(new S2EMaterialRequestSampleApproveTrans_TB
                            {
                                RMREQSAMID = RMReqSamID,
                                RMREQSAMLINEID = RMReqSamLineID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1
                            });
                        }

                        var currentRecord = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await RMReqSampleLineRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByRMReqSampleID(RMReqSamID, RMReqSamLineID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await RMReqSampleAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = RequestDate;
                            await RMReqSampleAppTranRepo.UpdateAsync(AppTransByID);

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

                            var BodyEmail = "";
                            BodyEmail = $@"
                                <b> {PlantBodyEmail} </b> <br/>
                                <b> ใบเบิกวัตถุดิบ (LAB Sample) </b> <br/>
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
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/MaterialRequestSampleTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                <br/>
                            ";

                            var sendEmail = _emailService.SendEmail(
                                   $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ (LAB Sample)/ No.: {RMReqSampleLineByID.NO} ",
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
                    AlertSuccess = "แก้ไขใบเบิกวัตถุดิบ (LAB Sample) สำเร็จ";
                    return Redirect($"/S2E/Qtech/MaterialRequestSample/{RMReqSamID}/RequestDetail");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Qtech/MaterialRequestSample/{RMReqSamLineID}/Edit");
            }
        }
    }
}
