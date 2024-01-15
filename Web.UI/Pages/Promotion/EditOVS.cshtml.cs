using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion
{
    public class EditOVSModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public PromotionDOMTable Promotion_OVS { get; set; }
        public List<PromotionAttachFileTable> AttachFile { get; set; }
        public List<SelectListItem> PromotionTypeByProductMaster { get; set; }
        public List<SelectListItem> PromotionTypeByMaster { get; set; }
        public List<SelectListItem> PromotionGroupCustomerMaster { get; set; }
        public List<SelectListItem> PromotionTypeFromByMaster { get; set; }
        public List<SelectListItem> PromotionTransMaster { get; set; }

        [BindProperty]
        public string FileLeft_1 { get; set; }
        [BindProperty]
        public string FileLeft_2 { get; set; }
        [BindProperty]
        public string FileLeft_3 { get; set; }
        public IFormFile UploadFileLeft_1 { get; set; }
        public IFormFile UploadFileLeft_2 { get; set; }
        public IFormFile UploadFileLeft_3 { get; set; }
        [BindProperty]
        public string TypeOVS { get; set; }
        public int BtnType { get; set; }
        public int statusEdit { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public EditOVSModel(
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

        private async Task InitialDataAsync(int id)
        {
            Promotion_OVS = new PromotionDOMTable();
            PromotionTypeByProductMaster = await GetPromotionTypeByProductMasterAsync();
            PromotionTypeByMaster = await GetPromotionTypeByMasterAsync();
            PromotionGroupCustomerMaster = await GetPromotionGroupCustomerMasterAsync();
            PromotionTypeFromByMaster = await GetPromotionTypeFromByMasterAsync();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionDOMRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                var dom = await promotionDOMRepo.GetAsync(id);

                Promotion_OVS = dom;
                
                var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                AttachFile = new List<PromotionAttachFileTable>();
                var attachFile = await unitOfWork.Promotion.GetFileByCCIdAsync(id, nameof(RequestTypeModel.OVS));

                AttachFile = attachFile.ToList();
                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await InitialDataAsync(id);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Promotion");
            }

        }

        public async Task<IActionResult> OnPostAsync(int id,string draft,string save)
        {
            try
            {
                // if (!ModelState.IsValid)
                // {
                //     await InitialDataAsync(id);

                //     return Redirect($@"/Promotion/{id}/EditOVS");
                // }
                if (!string.IsNullOrEmpty(draft))
                {
                    BtnType = 8;
                    statusEdit = 8;
                }
                if (!string.IsNullOrEmpty(save))
                {
                    BtnType = 1;
                    statusEdit = 3;
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var promotionRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var promotion = await promotionRepo.GetAsync(id);
                    var attachFileRepo = new GenericRepository<PromotionAttachFileTable>(unitOfWork.Transaction);
                    var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var requestdate = DateTime.Now;
                    // promotion.TypeOfProduct = Promotion_OVS.TypeOfProduct;
                    promotion.Pattern = Promotion_OVS.Pattern;
                    promotion.CustomerName = Promotion_OVS.CustomerName;
                    promotion.TypeOf = Promotion_OVS.TypeOf;
                    promotion.TypeOfRemark = Promotion_OVS.TypeOfRemark;
                    promotion.CustomerGroup = Promotion_OVS.CustomerGroup;
                    promotion.CustomerGroupRemark = Promotion_OVS.CustomerGroupRemark;
                    promotion.PaymentType = Promotion_OVS.PaymentType;
                    promotion.FromDate = Promotion_OVS.FromDate;
                    promotion.ToDate = Promotion_OVS.ToDate;
                    promotion.TypeOfRemark = Promotion_OVS.TypeOfRemark;
                    promotion.TypeFrom = Promotion_OVS.TypeFrom;
                    promotion.TypeFromRemark = Promotion_OVS.TypeFromRemark;
                    promotion.Objective = Promotion_OVS.Objective;
                    promotion.SalesPresentBath = Promotion_OVS.SalesPresentBath;
                    promotion.SalesForecastBath = Promotion_OVS.SalesForecastBath;
                    promotion.SalesChangeBath = Promotion_OVS.SalesChangeBath;
                    promotion.SalesRemarkBath = Promotion_OVS.SalesRemarkBath;
                    promotion.SalesPresentQty = Promotion_OVS.SalesPresentQty;
                    promotion.SalesForecastQty = Promotion_OVS.SalesForecastQty;
                    promotion.SalesChangeQty = Promotion_OVS.SalesChangeQty;
                    promotion.SalesRemarkQty = Promotion_OVS.SalesRemarkQty;
                    promotion.BudgetPresent = Promotion_OVS.BudgetPresent;
                    promotion.BudgetForecast = Promotion_OVS.BudgetForecast;
                    promotion.BudgetChange = Promotion_OVS.BudgetChange;
                    promotion.BudgetRemark = Promotion_OVS.BudgetRemark;
                    promotion.BudgetPresentBath = Promotion_OVS.BudgetPresentBath;
                    promotion.BudgetForecastBath = Promotion_OVS.BudgetForecastBath;
                    promotion.BudgetChangeBath = Promotion_OVS.BudgetChangeBath;
                    promotion.BudgetRemarkBath = Promotion_OVS.BudgetRemarkBath;
                    promotion.GetDiscount = Promotion_OVS.GetDiscount;
                    promotion.GetPoint = Promotion_OVS.GetPoint;
                    promotion.PromotionConditions = Promotion_OVS.PromotionConditions;
                    // promotion.RequestStatus = RequestStatusModel.WaitingForApprove;

                    await promotionRepo.UpdateAsync(promotion);

                    // Upload file.
                    string basePath = $"wwwroot/files/promotion/ovs/{id}";

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    var filePath = Path.GetTempFileName();

                    if (UploadFileLeft_1 != null && FileLeft_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLeft_1.FileName.Substring(UploadFileLeft_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLeft_1 + "." + TypeFile}"))
                        {
                            await UploadFileLeft_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = id,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLeft_1 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }

                    if (UploadFileLeft_2 != null && FileLeft_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLeft_2.FileName.Substring(UploadFileLeft_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLeft_2 + "." + TypeFile}"))
                        {
                            await UploadFileLeft_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = id,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLeft_2 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }

                    if (UploadFileLeft_3 != null && FileLeft_3 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileLeft_3.FileName.Substring(UploadFileLeft_3.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileLeft_3 + "." + TypeFile}"))
                        {
                            await UploadFileLeft_3.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = id,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 3,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLeft_3 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }
                    
                    if(BtnType == 1){
                        if(promotion.PromotionRef > 0){
                            TypeOVS = "OVS Excep";
                        }else{
                            TypeOVS = "OVS";
                        }

                        var approveMapping = await unitOfWork.Promotion.GetApproveGroupId(TypeOVS, _authService.GetClaim().UserId);
                        if (approveMapping == null)
                        {
                            throw new Exception("Approve mapping not match!");
                        }

                        var approveFlowAll = await approveFlowRepo.GetAllAsync();

                        var approveFlow = approveFlowAll.Where(x =>
                            x.ApproveMasterId == approveMapping.ApproveMasterId &&
                            x.IsActive == 1)
                            .OrderBy(x => x.ApproveLevel)
                            .ToList();

                        if (approveFlow.Count == 0)
                        {
                            throw new Exception("Approve flow not found.");
                        }

                        // var approveFlowAll = await approveFlowRepo.GetAllAsync();

                        // var approveFlow = approveFlowAll.Where(x =>
                        //     x.ApproveMasterId == approveMapping.ApproveMasterId &&
                        //     x.IsActive == 1 && x.ApproveLevel == promotion.CurrentApproveStep)
                        //     .OrderBy(x => x.ApproveLevel)
                        //     .ToList();

                        // if (approveFlow.Count == 0)
                        // {
                        //     throw new Exception("Approve flow not found.");
                        // }
                        if(promotion.RequestStatus!=4){
                            
                            // insert approve transaction
                            foreach (var item in approveFlow)
                            {
                                await approveTransRepo.InsertAsync(new PromotionApproveTransTable
                                {
                                    Email = item.Email,
                                    ApproveLevel = item.ApproveLevel,
                                    ApproveMasterId = item.ApproveMasterId,
                                    ApproveFlowId = item.Id,
                                    CCId = id,
                                    Position = item.Position,
                                    Status = item.Status,
                                    Name = item.Name,
                                    LastName = item.LastName,
                                });
                            }
                        
                        }
                        
                        // update approve step
                        var currentRecord = await promotionRepo.GetAsync(id);
                        // currentRecord.CurrentApproveStep = 1;
                        currentRecord.RequestStatus = statusEdit;
                        
                        await promotionRepo.UpdateAsync(currentRecord);

                        // update approve trans
                        var approveTransByCCId = await unitOfWork.Promotion.GetApproveTransByCCId(id);

                        var approveTransLevel1 = approveTransByCCId.Where(x => x.IsDone == 0).FirstOrDefault();
                        var approveLevelnext = approveTransLevel1.ApproveLevel;

                        var approveTrans = await approveTransRepo.GetAsync(approveTransLevel1.Id);

                        approveTrans.SendEmailDate = requestdate;

                        await approveTransRepo.UpdateAsync(approveTrans);

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = requestdate,
                            ExpireDate = requestdate.AddDays(30),
                            IsUsed = 0
                        });

                        if (promotion.PromotionRef > 0)
                        {
                            var refNumber = await promotionRepo.GetAsync(promotion.PromotionRef);

                            var sendEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {promotion.RequestNumber}",
                                $@"
                                    <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                    <b>ลูกค้า/Area : </b> {promotion.CustomerName}<br/>
                                    <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={id}&tid={approveTransLevel1.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                    <br/><b>เอกสารอ้างอิงเลขเดิม</b><br/>
                                    <b>เลขที่คำขอ : </b> {refNumber.RequestNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {refNumber.Pattern}<br/>
                                    <b>ลูกค้า/Area : </b> {refNumber.CustomerName}<br/>
                                    <b>Link อ้างอิง:</b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/{promotion.PromotionRef}/Render'>คลิกที่นี่</a> <br/>
                                ",
                                new List<string> { approveTrans.Email },
                                new List<string> { },
                                _authService.GetClaim().Email
                            );
                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }
                        }
                        else
                        {
                            var sendEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {promotion.RequestNumber}",
                                $@"
                                    <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                    <b>ลูกค้า/Area : </b> {promotion.CustomerName}<br/>
                                    <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={id}&tid={approveTransLevel1.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                ",
                                new List<string> { approveTrans.Email },
                                new List<string> { },
                                _authService.GetClaim().Email
                            );
                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }
                        }

                    }
                    // var approveTrans = await approveTransRepo.GetAsync(id);
                    // var approveTransAll = await approveTransRepo.GetAllAsync();
                    // var approvePromotion = approveTransAll.Where(x =>
                    //     x.CCId == id
                    //     && x.IsDone == 0
                    //     && x.SendEmailDate != null
                    // ).FirstOrDefault();
                    // var approveLevelnext = approvePromotion.ApproveLevel;

                    // // generate nonce
                    // var nonceKey = Guid.NewGuid().ToString();

                    // await nonceRepo.InsertAsync(new NonceTable
                    // {
                    //     NonceKey = nonceKey,
                    //     CreateDate = DateTime.Now,
                    //     ExpireDate = DateTime.Now.AddDays(30),
                    //     IsUsed = 0
                    // });

                    // var nextApproveTrans = await unitOfWork.Promotion.GetApproveTransByLevel(id, approveLevelnext);

                    // nextApproveTrans.SendEmailDate = DateTime.Now;
                    // nextApproveTrans.RejectDate = null;
                    // await approveTransRepo.UpdateAsync(nextApproveTrans);

                    // if (promotion.PromotionRef > 0 && Regex.IsMatch(Promotion_OVS.RequestNumber, "Excep?"))
                    // {
                    //     var refNumber = await promotionRepo.GetAsync((int)promotion.PromotionRef);
                    //     var sendEmail = _emailService.SendEmail(
                    //         $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {Promotion_OVS.RequestNumber}",
                    //         $@"
                    //             <b>เลขที่คำขอ:</b> {Promotion_OVS.RequestNumber}<br/>
                    //             <b>ชื่อเรื่อง:</b> {Promotion_OVS.Pattern}<br/>
                    //             <b>ชื่อลูกค้า:</b> {Promotion_OVS.CustomerName}<br/>
                    //             <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={id}&tid={approvePromotion.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
                    //             <br/><b>เอกสารอ้างอิงเลขเดิม</b><br/>
                    //             <b>เลขที่คำขอ : </b> {refNumber.RequestNumber}<br/>
                    //             <b>ชื่อเรื่อง : </b> {refNumber.Pattern}<br/>
                    //             <b>ลูกค้า/Area : </b> {refNumber.CustomerName}<br/>
                    //             <b>Link อ้างอิง:</b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/{id}/Render'>คลิกที่นี่</a> <br/>
                    //         ",
                    //         new List<string> { approvePromotion.Email },
                    //         new List<string> { },
                    //         _authService.GetClaim().Email
                    //     );
                    //     if (sendEmail.Result == false)
                    //     {
                    //         throw new Exception(sendEmail.Message);
                    //     }
                    // }
                    // else
                    // {
                    //     var sendEmail = _emailService.SendEmail(
                    //         $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {Promotion_OVS.RequestNumber}",
                    //         $@"
                    //             <b>เลขที่คำขอ : </b> {Promotion_OVS.RequestNumber}<br/>
                    //             <b>ชื่อเรื่อง : </b> {Promotion_OVS.Pattern}<br/>
                    //             <b>ชื่อลูกค้า : </b> {Promotion_OVS.CustomerName}<br/>
                    //             <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={id}&tid={approvePromotion.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                    //         ",
                    //         new List<string> { approvePromotion.Email },
                    //         new List<string> { },
                    //         _authService.GetClaim().Email
                    //     );

                    //     if (sendEmail.Result == false)
                    //     {
                    //         throw new Exception(sendEmail.Message);
                    //     }
                    // }


                    unitOfWork.Complete();
                    AlertSuccess = "Edit Success.";
                    return Redirect($@"/Promotion");
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect($@"/Promotion/{id}/EditOVS");
            }
        }

        public async Task<IActionResult> OnGetDeleteFileAsync(int IdFile)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<PromotionAttachFileTable>(unitOfWork.Transaction);

                    var attachFile = await attachFileRepo.GetAsync(IdFile);
                    string basePath = attachFile.FilePath + "/" + attachFile.FileName;

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.File.Delete(basePath);
                        await attachFileRepo.DeleteAsync(new PromotionAttachFileTable
                        {
                            Id = IdFile
                        });
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteFile = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }
        public async Task<List<SelectListItem>> GetPromotionTypeByProductMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var customerTypeByProductRepo = new GenericRepository<PromotionByProductTable>(unitOfWork.Transaction);

                var customerTypeByProdictAll = await customerTypeByProductRepo.GetAllAsync();

                unitOfWork.Complete();

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
                    .Where(x => x.ByType == "OVS")
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
                    .Where(x => x.ByType == "OVS")
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