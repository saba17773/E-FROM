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

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterialSample
{
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int AddRMSampleId { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public string DealerAddress { get; set; }
        [BindProperty]
        public string ProductionSite { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public EditModel(
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
        public async Task<IActionResult> OnGetAsync(int AddRMSampleID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_ADDRAWMATERIALSAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    AddRMSampleId = AddRMSampleID;
                    await GetData(AddRMSampleID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/AddRawMaterialSample/Main");
            }
        }

        public async Task GetData(int AddRMSampleID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AddRMSampleByID.ASSESSMENTID);

                var RequestID = RMAssessmentByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                VendorID = NewRequestByID.VENDORID;

                SupplierName = NewRequestByID.SUPPLIERNAME;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                ItemName = NewRequestByID.ITEMNAME;
                Price = NewRequestByID.PRICE;
                Unit = NewRequestByID.UNIT;
                Qty = NewRequestByID.QTY;
                CurrencyCode = NewRequestByID.CURRENCYCODE;
                Plant = NewRequestByID.PLANT;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int AddRMSampleID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                AddRMSampleId = AddRMSampleID;
                await GetData(AddRMSampleID);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var AssessmentID = AddRMSampleByID.ASSESSMENTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.PLANT == NewRequestByID.PLANT &&
                                                                   x.ISADDRMSAMPLE == 1
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

                    //UPDATE ADD RM Sample
                    AddRMSampleByID.PLANT = NewRequestByID.PLANT;
                    AddRMSampleByID.APPROVEGROUPID = ApproveGroupID;
                    AddRMSampleByID.APPROVEMASTERID = approvemasterid;
                    AddRMSampleByID.APPROVESTATUS = ApproveStatus;
                    AddRMSampleByID.UPDATEBY = CreateBy;
                    AddRMSampleByID.UPDATEDATE = CreateDate;

                    await AddRMSampleRepo.UpdateAsync(AddRMSampleByID);

                    if (!string.IsNullOrEmpty(save))
                    {
                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.ADDRMSAMPLEID == AddRMSampleID && x.APPROVEGROUPID == ApproveGroupID);
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
                        var nonceRepo = new GenericRepository<S2EAddRawMaterialSampleNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2EAddRawMaterialSampleNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = CreateDate,
                            EXPIREDATE = CreateDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT PC APPROVE TRANSECTION
                        var AddRMSampleAppTranRepo = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {
                            await AddRMSampleAppTranRepo.InsertAsync(new S2EAddRawMaterialSampleApproveTrans_TB
                            {
                                ADDRMSAMPLEID = AddRMSampleID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove
                            });
                        }

                        var currentRecord = await AddRMSampleRepo.GetAsync(AddRMSampleByID.ID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await AddRMSampleRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByAddRMSampleID(AddRMSampleID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);

                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                        x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await AddRMSampleAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = CreateDate;
                            await AddRMSampleAppTranRepo.UpdateAsync(AppTransByID);

                            var sendEmail = _emailService.SendEmail(
                                  $"{RequestCode} / เพื่อแจ้งให้หน่วยงาน Store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง (LAB Sample)",
                                   $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย </b><br/>
                                    <table style='text-align:left;'>
                                        <tr>
                                            <td style='text-align:right;'>Request Code : </td>
                                            <td>{NewRequestByID.REQUESTCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>รหัสผู้ขาย/ผู้ผลิต : </td>
                                            <td>{NewRequestByID.VENDORID}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{NewRequestByID.SUPPLIERNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ที่อยู่ของตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>Item Name : </td>
                                            <td colspan='3'>{NewRequestByID.ITEMNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ราคา : </td>
                                            <td>{String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)}  {NewRequestByID.CURRENCYCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>จำนวน : </td>
                                            <td>{String.Format("{0:#,##0.#0}", NewRequestByID.QTY)}</td>
                                            <td style='text-align:right;'>หน่วย : </td>
                                            <td>{NewRequestByID.UNIT}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>สถานที่จัดเก็บ : </td>
                                            <td> {NewRequestByID.PLANT} </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialSampleTodolist?Email={AppTrans.EMAIL}'>  คลิกที่นี่ </a> <br/>
                                ",
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

                    AlertSuccess = "แก้ไขใบร้องขอเพิ่มวัตถุดิบเข้าระบบ (LAB Sample) สำเร็จ";
                    return Redirect("/S2E/Purchase/AddRawMaterialSample/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/AddRawMaterialSample/{AddRMSampleID}/Edit");
            }
        }
    }
}
