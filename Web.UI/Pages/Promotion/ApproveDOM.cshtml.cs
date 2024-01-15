using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.Promotion;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion
{
    public class ApproveDOMModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public PromotionDOMTable Promotion_DOM { get; set; }

        public string TypeOfProduct { get; set; }
        public string TypeOf { get; set; }
        public string CustomerGroup { get; set; }
        public string TypeFrom { get; set; }
        public List<PromotionAttachFileTable> AttachFile { get; set; }
        public List<PromotionAttachFileTable> AttachFileRef { get; set; }

        [BindProperty]
        public string FileLevel1_1 { get; set; }
        [BindProperty]
        public string FileLevel1_2 { get; set; }
        public IFormFile UploadFileLevel1_1 { get; set; }
        public IFormFile UploadFileLevel1_2 { get; set; }
        [BindProperty]
        public string FileLevel2_1 { get; set; }
        [BindProperty]
        public string FileLevel2_2 { get; set; }
        public IFormFile UploadFileLevel2_1 { get; set; }
        public IFormFile UploadFileLevel2_2 { get; set; }
        [BindProperty]
        public string FileLevel3_1 { get; set; }
        [BindProperty]
        public string FileLevel3_2 { get; set; }
        public IFormFile UploadFileLevel3_1 { get; set; }
        public IFormFile UploadFileLevel3_2 { get; set; }
        [BindProperty]
        public string FileLevel4_1 { get; set; }
        [BindProperty]
        public string FileLevel4_2 { get; set; }
        public IFormFile UploadFileLevel4_1 { get; set; }
        public IFormFile UploadFileLevel4_2 { get; set; }
        [BindProperty]
        public string FileLevel5_1 { get; set; }
        [BindProperty]
        public string FileLevel5_2 { get; set; }
        public IFormFile UploadFileLevel5_1 { get; set; }
        public IFormFile UploadFileLevel5_2 { get; set; }
        [BindProperty]
        public string FileLevel6_1 { get; set; }
        [BindProperty]
        public string FileLevel6_2 { get; set; }
        public IFormFile UploadFileLevel6_1 { get; set; }
        public IFormFile UploadFileLevel6_2 { get; set; }
        [BindProperty]
        public string FileLevel7_1 { get; set; }
        [BindProperty]
        public string FileLevel7_2 { get; set; }
        public IFormFile UploadFileLevel7_1 { get; set; }
        public IFormFile UploadFileLevel7_2 { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }
        [BindProperty]
        [Required]
        public int ApproveResult { get; set; }
        public int DOMLevel { get; set; }
        public int IsFile { get; set; }
        [BindProperty]
        public string Remark1 { get; set; }
        [BindProperty]
        public string Remark2 { get; set; }
        [BindProperty]
        public string Remark3 { get; set; }
        [BindProperty]
        public string Remark4 { get; set; }
        [BindProperty]
        public string Remark5 { get; set; }
        [BindProperty]
        public string Remark6 { get; set; }
        [BindProperty]
        public string Remark7 { get; set; }
        [BindProperty]
        public string NameL1 { get; set; }
        [BindProperty]
        public string NameL2 { get; set; }
        [BindProperty]
        public string NameL3 { get; set; }
        [BindProperty]
        public string NameL4 { get; set; }
        [BindProperty]
        public string NameL5 { get; set; }
        [BindProperty]
        public string NameL6 { get; set; }
        [BindProperty]
        public string NameL7 { get; set; }
        public Double SalesPresentBath { get; set; }
        public Double SalesForecastBath { get; set; }
        public Double SalesChangeBath { get; set; }
        public Double SalesPresentQty { get; set; }
        public Double SalesForecastQty { get; set; }
        public Double SalesChangeQty { get; set; }
        public Double BudgetPresent { get; set; }
        public Double BudgetForecast { get; set; }
        public Double BudgetChange { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ApproveDOMModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          ICreditControlService creditControlService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _creditControlService = creditControlService;
            _emailService = emailService;
            _configuration = configuration;
        }

        private async Task InitialDataAsync(int id, int tid, int level)
        {
            Promotion_DOM = new PromotionDOMTable();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                var typeOfProductRepo = new GenericRepository<PromotionByProductTable>(unitOfWork.Transaction);
                var typeOfRepo = new GenericRepository<PromotionTypeTable>(unitOfWork.Transaction);
                var customerGroupRepo = new GenericRepository<PromotionGroupCustomerTable>(unitOfWork.Transaction);
                var typeFromRepo = new GenericRepository<PromotionTypeFromTable>(unitOfWork.Transaction);
                var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                AttachFile = new List<PromotionAttachFileTable>();
                AttachFileRef = new List<PromotionAttachFileTable>();

                var promotion = await promotionRepo.GetAsync(id);
                var typeOfProduct = await typeOfProductRepo.GetAsync((int)promotion.TypeOfProduct);
                var typeOf = await typeOfRepo.GetAsync((int)promotion.TypeOf);
                var customerGroup = await customerGroupRepo.GetAsync((int)promotion.CustomerGroup);
                var typeFrom = await typeFromRepo.GetAsync((int)promotion.TypeFrom);

                // remark level
                var approveTrans = await approveTransRepo.GetAllAsync();
                var countLevel = approveTrans.Where(x =>
                    x.CCId == id
                ).ToList().Count;

                // name level
                var approveTransFlow = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == level)
                    .FirstOrDefault();
                var approveFlow = await approveFlowRepo.GetAllAsync();
                var isLevel = approveFlow.Where(x =>
                    x.ApproveMasterId == approveTransFlow.ApproveMasterId && x.ApproveLevel == level
                ).FirstOrDefault();
                IsFile = isLevel.IsFile;

                if (countLevel >= 1)
                {
                    var remark1 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 1)
                    .FirstOrDefault();

                    var namel1 = approveFlow.Where(x =>
                    x.ApproveMasterId == remark1.ApproveMasterId &&
                    x.ApproveLevel == 1)
                    .FirstOrDefault();

                    Remark1 = remark1.Remark;
                    NameL1 = namel1.Name;
                }

                if (countLevel >= 2)
                {
                    var remark2 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 2)
                    .FirstOrDefault();

                    var namel2 = approveFlow.Where(x =>
                   x.ApproveMasterId == remark2.ApproveMasterId &&
                   x.ApproveLevel == 2)
                   .FirstOrDefault();

                    Remark2 = remark2.Remark;
                    NameL2 = namel2.Name;
                }

                if (countLevel >= 3)
                {
                    var remark3 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 3)
                    .FirstOrDefault();

                    var namel3 = approveFlow.Where(x =>
                    x.ApproveMasterId == remark3.ApproveMasterId &&
                    x.ApproveLevel == 3)
                    .FirstOrDefault();

                    Remark3 = remark3.Remark;
                    NameL3 = namel3.Name;
                }

                if (countLevel >= 4)
                {
                    var remark4 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 4)
                    .FirstOrDefault();

                    var namel4 = approveFlow.Where(x =>
                    x.ApproveMasterId == remark4.ApproveMasterId &&
                    x.ApproveLevel == 4)
                    .FirstOrDefault();

                    Remark4 = remark4.Remark;
                    NameL4 = namel4.Name;
                }

                if (countLevel >= 5)
                {
                    var remark5 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 5)
                    .FirstOrDefault();

                    var namel5 = approveFlow.Where(x =>
                    x.ApproveMasterId == remark5.ApproveMasterId &&
                    x.ApproveLevel == 5)
                    .FirstOrDefault();

                    Remark5 = remark5.Remark;
                    NameL5 = namel5.Name;
                }

                if (countLevel >= 6)
                {
                    var remark6 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 6)
                    .FirstOrDefault();

                    var namel6 = approveFlow.Where(x =>
                    x.ApproveMasterId == remark6.ApproveMasterId &&
                    x.ApproveLevel == 6)
                    .FirstOrDefault();

                    Remark6 = remark6.Remark;
                    NameL6 = namel6.Name;
                }

                if (countLevel >= 7)
                {
                    var remark7 = approveTrans.Where(x =>
                    x.CCId == id &&
                    x.ApproveLevel == 7)
                    .FirstOrDefault();

                    var namel7 = approveFlow.Where(x =>
                    x.ApproveMasterId == remark7.ApproveMasterId &&
                    x.ApproveLevel == 7)
                    .FirstOrDefault();

                    Remark7 = remark7.Remark;
                    NameL7 = namel7.Name;
                }

                Promotion_DOM = promotion;

                Promotion_DOM.RequestNumber = promotion.RequestNumber;
                Promotion_DOM.RequestDate = promotion.RequestDate;
                TypeOfProduct = typeOfProduct.ByCode + " (" + typeOfProduct.ByName + ") ";
                Promotion_DOM.Pattern = promotion.Pattern;
                Promotion_DOM.CustomerName = promotion.CustomerName;
                TypeOf = typeOf.PromotionType;
                Promotion_DOM.TypeOfRemark = promotion.TypeOfRemark;
                CustomerGroup = customerGroup.GroupName;
                Promotion_DOM.CustomerGroupRemark = promotion.CustomerGroupRemark;
                Promotion_DOM.PaymentType = promotion.PaymentType;
                Promotion_DOM.FromDate = promotion.FromDate;
                Promotion_DOM.ToDate = promotion.ToDate;
                TypeFrom = typeFrom.PromotionFrom;
                Promotion_DOM.TypeFromRemark = promotion.TypeFromRemark;
                Promotion_DOM.Objective = promotion.Objective;
                // Promotion_DOM.SalesPresentBath = promotion.SalesPresentBath;
                // Promotion_DOM.SalesForecastBath = promotion.SalesForecastBath;
                // Promotion_DOM.SalesChangeBath = promotion.SalesChangeBath;
                Promotion_DOM.SalesRemarkBath = promotion.SalesRemarkBath;
                // Promotion_DOM.SalesPresentQty = promotion.SalesPresentQty;
                // Promotion_DOM.SalesForecastQty = promotion.SalesForecastQty;
                // Promotion_DOM.SalesChangeQty = promotion.SalesChangeQty;
                Promotion_DOM.SalesRemarkQty = promotion.SalesRemarkQty;
                // Promotion_DOM.BudgetPresent = promotion.BudgetPresent;
                // Promotion_DOM.BudgetForecast = promotion.BudgetForecast;
                // Promotion_DOM.BudgetChange = promotion.BudgetChange;
                Promotion_DOM.BudgetRemark = promotion.BudgetRemark;
                
                SalesPresentBath = promotion.SalesPresentBath;
                SalesForecastBath = promotion.SalesForecastBath;
                SalesChangeBath = promotion.SalesChangeBath;
                SalesPresentQty = promotion.SalesPresentQty;
                SalesForecastQty = promotion.SalesForecastQty;
                SalesChangeQty = promotion.SalesChangeQty;
                BudgetPresent = promotion.BudgetPresent;
                BudgetForecast = promotion.BudgetForecast;
                BudgetChange = promotion.BudgetChange;
                
                Promotion_DOM.GetDiscount = promotion.GetDiscount;
                Promotion_DOM.GetPoint = promotion.GetPoint;
                Promotion_DOM.PromotionConditions = promotion.PromotionConditions;
                DOMLevel = level;
                IsFile = IsFile;

                var attachFile = await unitOfWork.Promotion.GetFileByCCIdAsync(id, nameof(RequestTypeModel.DOM));
                AttachFile = attachFile.ToList();

                var attachFileref = await unitOfWork.Promotion.GetFileByCCIdAsync(promotion.PromotionRef, nameof(RequestTypeModel.DOM));
                AttachFileRef = attachFileref.ToList();

                unitOfWork.Complete();
            }
        }

        public async Task OnGet(int id, int tid, int level, string nonce)
        {
            await InitialDataAsync(id, tid, level);
        }

        public async Task<IActionResult> OnPostAsync(int id, int tid, int level, string nonce)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะ อนุมัติ หรือ ไม่อนุมัติ";
                }

                if (!ModelState.IsValid)
                {
                    return Redirect($"/Result");
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                    var promotionRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<PromotionAttachFileTable>(unitOfWork.Transaction);
                    var requestdate = DateTime.Now;

                    // check nonce
                    var _nonce = await unitOfWork.Nonce.GetNonceByKey(nonce);

                    if (_nonce.ExpireDate <= requestdate || _nonce.IsUsed == 1)
                    {
                        throw new Exception("Nonce expired.");
                    }

                    _nonce.IsUsed = 1;

                    var approveTrans = await approveTransRepo.GetAsync(tid);
                    var promotion = await promotionRepo.GetAsync(id);
                    var approveTransAll = await unitOfWork.Promotion.GetApproveTransByCCId(id);

                    // update approve trans
                    approveTrans.IsDone = 1;
                    approveTrans.Remark = ApproveRemark;

                    if (ApproveResult == 1)
                    {
                        approveTrans.ApproveDate = requestdate;
                        promotion.RequestStatus = RequestStatusModel.WaitingForApprove;
                    }
                    else if (ApproveResult == 2)
                    {
                        approveTrans.RejectDate = requestdate;
                        promotion.RequestStatus = RequestStatusModel.Reject;
                        approveTrans.IsDone = 0;
                    }

                    // is final approve ?
                    if (promotion.CurrentApproveStep == approveTransAll.ToList().Count)
                    {
                        if (ApproveResult == 1)
                        {
                            promotion.RequestStatus = RequestStatusModel.Complete;

                            var nextApproveTrans = await unitOfWork.Promotion.GetApproveTransByLevel(id, promotion.CurrentApproveStep);

                            var createBy = await unitOfWork.Promotion.GetCreateBy(id);

                            var sendCompleteEmail = _emailService.SendEmail(
                                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {promotion.RequestNumber}",
                                    $@"
                                    <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                    <b>ลูกค้า/Area : </b> {promotion.CustomerName}<br/>
                                    <b>ดำเนินการเสร็จสิ้น</b>
                                ",
                                    new List<string> { createBy.Email },
                                    new List<string> { },
                                    nextApproveTrans.Email
                                );

                            if (sendCompleteEmail.Result == false)
                            {
                                throw new Exception(sendCompleteEmail.Message);
                            }
                        }
                        else
                        {
                            var promotionTrans = await unitOfWork.Promotion.GetCreateBy(id);
                            var approveTransEmail = await approveTransRepo.GetAllAsync();
                            var email = approveTransEmail.Where(x =>
                                x.CCId == id &&
                                x.ApproveLevel == level)
                            .FirstOrDefault();

                            var sendRejectEmail = _emailService.SendEmail(
                                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {promotion.RequestNumber}",
                                    $@"
                                        <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                        <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                        <b>ชื่อลูกค้า : </b> {promotion.CustomerName}<br/>
                                        <b>สถานะ : </b> ไม่อนุมัติ<br/>
                                        <b>หมายเหตุ : </b> {ApproveRemark}<br/>
                                    ",
                                    new List<string> { promotionTrans.Email },
                                    new List<string> { },
                                    email.Email
                                );
                            if (sendRejectEmail.Result == false)
                            {
                                throw new Exception(sendRejectEmail.Message);
                            }
                        }

                    }
                    else
                    {
                        // update head table
                        var nextApproveTrans = await unitOfWork.Promotion.GetApproveTransByLevel(id, promotion.CurrentApproveStep);
                        if (ApproveResult == 1)
                        {
                            promotion.CurrentApproveStep += 1;
                            nextApproveTrans = await unitOfWork.Promotion.GetApproveTransByLevel(id, promotion.CurrentApproveStep);
                        }

                        // next approve trans


                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = requestdate,
                            ExpireDate = requestdate.AddDays(30),
                            IsUsed = 0
                        });

                        var approveTransEmail = await approveTransRepo.GetAllAsync();
                        var email = approveTransEmail.Where(x =>
                            x.CCId == id &&
                            x.ApproveLevel == level)
                            .FirstOrDefault();

                        if (ApproveResult == 2)
                        {
                            var promotionTrans = await unitOfWork.Promotion.GetCreateBy(id);
                            var sendRejectEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {promotion.RequestNumber}",
                                $@"
                                    <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                    <b>ชื่อลูกค้า : </b> {promotion.CustomerName}<br/>
                                    <b>สถานะ : </b> ไม่อนุมัติ<br/>
                                    <b>หมายเหตุ : </b> {ApproveRemark}<br/>
                                ",
                                new List<string> { promotionTrans.Email },
                                new List<string> { },
                                email.Email
                            );
                            if (sendRejectEmail.Result == false)
                            {
                                throw new Exception(sendRejectEmail.Message);
                            }
                        }

                        if (ApproveResult == 1)
                        {
                            // promotion.CurrentApproveStep += 1;
                            // check email referance
                            if (promotion.PromotionRef > 0 && Regex.IsMatch(promotion.RequestNumber, "Excep?"))
                            {
                                var refNumber = await promotionRepo.GetAsync((int)promotion.PromotionRef);
                                var sendNextEmail = _emailService.SendEmail(
                                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {promotion.RequestNumber}",
                                    $@"
                                        <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                        <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                        <b>ชื่อลูกค้า : </b> {promotion.CustomerName}<br/>
                                        <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveDOM?id={promotion.Id}&tid={nextApproveTrans.Id}&level={level + 1}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
                                        <br/><b>เอกสารอ้างอิงเลขเดิม</b><br/>
                                        <b>เลขที่คำขอ : </b> {refNumber.RequestNumber}<br/>
                                        <b>ชื่อเรื่อง : </b> {refNumber.Pattern}<br/>
                                        <b>ลูกค้า/Area : </b> {refNumber.CustomerName}<br/>
                                        <b>Link อ้างอิง : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/{promotion.Id}/Render'>คลิกที่นี่</a> <br/>
                                    ",
                                    new List<string> { nextApproveTrans.Email },
                                    new List<string> { },
                                    email.Email
                                );
                                if (sendNextEmail.Result == false)
                                {
                                    throw new Exception(sendNextEmail.Message);
                                }
                            }
                            else
                            {
                                var sendNextEmail = _emailService.SendEmail(
                                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {promotion.RequestNumber}",
                                    $@"
                                        <b>เลขที่คำขอ:</b> {promotion.RequestNumber}<br/>
                                        <b>ชื่อเรื่อง:</b> {promotion.Pattern}<br/>
                                        <b>ชื่อลูกค้า:</b> {promotion.CustomerName}<br/>
                                        <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveDOM?id={promotion.Id}&tid={nextApproveTrans.Id}&level={level + 1}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
                                    ",
                                    new List<string> { nextApproveTrans.Email },
                                    new List<string> { },
                                    email.Email
                                );
                                if (sendNextEmail.Result == false)
                                {
                                    throw new Exception(sendNextEmail.Message);
                                }
                            }
                        }

                        nextApproveTrans.SendEmailDate = requestdate;

                        await approveTransRepo.UpdateAsync(nextApproveTrans);
                    }

                    await approveTransRepo.UpdateAsync(approveTrans);
                    await promotionRepo.UpdateAsync(promotion);
                    await nonceRepo.UpdateAsync(_nonce);

                    // Upload file.
                    string basePath = $"wwwroot/files/promotion/dom/{(int)promotion.Id}";

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    var filePath = Path.GetTempFileName();
                    // 1
                    if (UploadFileLevel1_1 != null && FileLevel1_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel1_1.FileName.Substring(UploadFileLevel1_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel1_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel1_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel1_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }

                    if (UploadFileLevel1_2 != null && FileLevel1_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel1_2.FileName.Substring(UploadFileLevel1_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel1_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel1_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel1_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    // 2
                    if (UploadFileLevel2_1 != null && FileLevel2_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel2_1.FileName.Substring(UploadFileLevel2_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel2_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel2_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel2_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }

                    if (UploadFileLevel2_2 != null && FileLevel2_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel2_2.FileName.Substring(UploadFileLevel2_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel2_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel2_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel2_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    // 3
                    if (UploadFileLevel3_1 != null && FileLevel3_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel3_1.FileName.Substring(UploadFileLevel3_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel3_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel3_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel3_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }

                    if (UploadFileLevel3_2 != null && FileLevel3_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel3_2.FileName.Substring(UploadFileLevel3_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel3_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel3_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel3_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    // 4
                    if (UploadFileLevel4_1 != null && FileLevel4_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel4_1.FileName.Substring(UploadFileLevel4_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel4_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel4_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel4_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    if (UploadFileLevel4_2 != null && FileLevel4_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel4_2.FileName.Substring(UploadFileLevel4_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel4_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel4_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel4_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    // 5
                    if (UploadFileLevel5_1 != null && FileLevel5_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel5_1.FileName.Substring(UploadFileLevel5_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel5_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel5_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel5_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    if (UploadFileLevel5_2 != null && FileLevel5_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel5_2.FileName.Substring(UploadFileLevel5_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel5_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel5_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel5_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    // 6
                    if (UploadFileLevel6_1 != null && FileLevel6_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel6_1.FileName.Substring(UploadFileLevel6_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel6_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel6_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel6_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    if (UploadFileLevel6_2 != null && FileLevel6_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel6_2.FileName.Substring(UploadFileLevel6_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel6_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel6_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel6_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    // 7
                    if (UploadFileLevel7_1 != null && FileLevel7_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel7_1.FileName.Substring(UploadFileLevel7_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel7_1 + "." + TypeFile}"))
                        {
                            await UploadFileLevel7_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel7_1 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }
                    if (UploadFileLevel7_2 != null && FileLevel7_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLevel7_2.FileName.Substring(UploadFileLevel7_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLevel7_2 + "." + TypeFile}"))
                        {
                            await UploadFileLevel7_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = promotion.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLevel7_2 + "." + TypeFile}",
                                FileLevel = level
                            });
                        }
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "ดำเนินการเสร็จสิ้น";
                    return Redirect($"/Result");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Result");
            }
        }

        public async Task<List<SelectListItem>> GetCustomerTypeByProductMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var customerTypeByProductRepo = new GenericRepository<PromotionByProductTable>(unitOfWork.Transaction);

                var customerTypeByProdictAll = await customerTypeByProductRepo.GetAllAsync();

                return customerTypeByProdictAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.ByCode + " (" + x.ByName + ")"
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPromotionTypeByMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionTypeRepo = new GenericRepository<PromotionTypeTable>(unitOfWork.Transaction);

                var promotionTypeAll = await promotionTypeRepo.GetAllAsync();

                return promotionTypeAll
                    .Where(x => x.ByType == "DOM")
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.PromotionType
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPromotionGroupCustomerMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionGroupCustomerRepo = new GenericRepository<PromotionGroupCustomerTable>(unitOfWork.Transaction);

                var promotionGroupCustomerAll = await promotionGroupCustomerRepo.GetAllAsync();

                return promotionGroupCustomerAll
                    .Where(x => x.ByType == "DOM")
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.GroupName
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPromotionTypeFromByMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionTypeFromRepo = new GenericRepository<PromotionTypeFromTable>(unitOfWork.Transaction);

                var promotionFromTypeAll = await promotionTypeFromRepo.GetAllAsync();

                return promotionFromTypeAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.PromotionFrom
                    })
                    .ToList();
            }
        }

    }
}