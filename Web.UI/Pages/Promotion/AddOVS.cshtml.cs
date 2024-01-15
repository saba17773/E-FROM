using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
using Org.BouncyCastle.Ocsp;
using Renci.SshNet.Messages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion
{
    public class AddOVSModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public PromotionDOMTable Promotion_OVS { get; set; }

        // file upload
        [BindProperty]
        public string FileLeft_1 { get; set; }
        [BindProperty]
        public string FileLeft_2 { get; set; }
        [BindProperty]
        public string FileLeft_3 { get; set; }
        [BindProperty]
        public string FileRight_1 { get; set; }
        [BindProperty]
        public string FileRight_2 { get; set; }
        [BindProperty]
        public string FileRight_3 { get; set; }
        public IFormFile UploadFileLeft_1 { get; set; }
        public IFormFile UploadFileLeft_2 { get; set; }
        public IFormFile UploadFileLeft_3 { get; set; }
        public IFormFile UploadFileRight_1 { get; set; }
        public IFormFile UploadFileRight_2 { get; set; }
        public IFormFile UploadFileRight_3 { get; set; }
        public List<SelectListItem> PromotionTypeByProductMaster { get; set; }
        public List<SelectListItem> PromotionTypeByMaster { get; set; }
        public List<SelectListItem> PromotionGroupCustomerMaster { get; set; }
        public List<SelectListItem> PromotionTypeFromByMaster { get; set; }
        public List<SelectListItem> PromotionTransMaster { get; set; }

        [BindProperty]
        public string TypeOVS { get; set; }
        public List<PromotionAttachFileTable> AttachFile { get; set; }
        public string listFile { get; set; }
        public int BtnType { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddOVSModel(
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

        private async Task InitialDataAsync()
        {
            Promotion_OVS = new PromotionDOMTable();
            PromotionTypeByProductMaster = await GetPromotionTypeByProductMasterAsync();
            PromotionTypeByMaster = await GetPromotionTypeByMasterAsync();
            PromotionGroupCustomerMaster = await GetPromotionGroupCustomerMasterAsync();
            PromotionTypeFromByMaster = await GetPromotionTypeFromByMasterAsync();
            PromotionTransMaster = await GetPromotionTransMasterAsync();
        }

        public async Task OnGetAsync()
        {
            await InitialDataAsync();
        }

        public async Task<IActionResult> OnPostAsync(string draft,string save)
        {
            try
            {
                await _authService.CanAccess(nameof(PromotionPermissionModel.ADD_OVS_PROMOTION));
                if (!ModelState.IsValid)
                {
                    await InitialDataAsync();

                    return Page();
                }
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var promotionRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<PromotionAttachFileTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var requestdate = DateTime.Now;

                    string prefixType;
                    int Proref;
                    string reqNumber;

                    if (!string.IsNullOrEmpty(draft))
                    {
                        BtnType = 8;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        BtnType = 1;
                    }

                    if (TypeOVS == "OVS")
                    {
                        var numberSeqRepo = new GenericRepository<PromotionByProductTable>(unitOfWork.Transaction);
                        var numberSeq = await numberSeqRepo.GetAllAsync();
                        var numberSeqOVS = numberSeq.Where(x => x.Id == Promotion_OVS.TypeOfProduct).FirstOrDefault();
                        numberSeqOVS.SeqValue += 1;
                        await numberSeqRepo.UpdateAsync(numberSeqOVS);
                        // prefixType = "OVS-Product";
                        prefixType = "OVS-" + numberSeqOVS.ByCode;
                        Proref = Promotion_OVS.PromotionRef;
                        reqNumber = _creditControlService.GetLatestRequestNumberPromotionAsync(numberSeqOVS.SeqValue.ToString(), prefixType);
                    }
                    else
                    {
                        var numberSeqRepo = new GenericRepository<NumberSeqTable>(unitOfWork.Transaction);
                        var numberSeq = await numberSeqRepo.GetAllAsync();
                        var numberSeqOVS = numberSeq.Where(x => x.SeqKey == nameof(NumberSeqModel.Promotion_OVS)).FirstOrDefault();
                        numberSeqOVS.SeqValue += 1;
                        await numberSeqRepo.UpdateAsync(numberSeqOVS);
                        prefixType = "OVS-Excep";
                        Proref = Promotion_OVS.PromotionRef;
                        reqNumber = _creditControlService.GetLatestRequestNumberPromotionAsync(numberSeqOVS.SeqValue.ToString(), prefixType);
                    }

                    // Add Ovs
                    var newOVS = new PromotionDOMTable
                    {
                        RequestNumber = reqNumber,
                        RequestDate = requestdate,
                        TypeOfProduct = Promotion_OVS.TypeOfProduct,
                        Pattern = Promotion_OVS.Pattern,
                        TypeOf = Promotion_OVS.TypeOf,
                        TypeOfRemark = Promotion_OVS.TypeOfRemark,
                        CustomerName = Promotion_OVS.CustomerName,
                        CustomerGroup = Promotion_OVS.CustomerGroup,
                        CustomerGroupRemark = Promotion_OVS.CustomerGroupRemark,
                        PaymentType = Promotion_OVS.PaymentType,
                        FromDate = Promotion_OVS.FromDate,
                        ToDate = Promotion_OVS.ToDate,
                        TypeFrom = Promotion_OVS.TypeFrom,
                        TypeFromRemark = Promotion_OVS.TypeFromRemark,
                        Objective = Promotion_OVS.Objective,
                        SalesPresentBath = Convert.ToDouble(SplitComma(Promotion_OVS.SalesPresentBath.ToString())),
                        SalesForecastBath = Convert.ToDouble(SplitComma(Promotion_OVS.SalesForecastBath.ToString())),
                        SalesChangeBath = Convert.ToDouble(SplitComma(Promotion_OVS.SalesChangeBath.ToString())),
                        SalesRemarkBath = Promotion_OVS.SalesRemarkBath,
                        SalesPresentQty = Convert.ToDouble(SplitComma(Promotion_OVS.SalesPresentQty.ToString())),
                        SalesForecastQty = Convert.ToDouble(SplitComma(Promotion_OVS.SalesForecastQty.ToString())),
                        SalesChangeQty = Convert.ToDouble(SplitComma(Promotion_OVS.SalesChangeQty.ToString())),
                        SalesRemarkQty = Promotion_OVS.SalesRemarkQty,
                        BudgetPresent = Convert.ToDouble(SplitComma(Promotion_OVS.BudgetPresent.ToString())),
                        BudgetForecast = Convert.ToDouble(SplitComma(Promotion_OVS.BudgetForecast.ToString())),
                        BudgetChange = Convert.ToDouble(SplitComma(Promotion_OVS.BudgetChange.ToString())),
                        BudgetRemark = Promotion_OVS.BudgetRemark,
                        BudgetPresentBath = Convert.ToDouble(SplitComma(Promotion_OVS.BudgetPresentBath.ToString())),
                        BudgetForecastBath = Convert.ToDouble(SplitComma(Promotion_OVS.BudgetForecastBath.ToString())),
                        // BudgetChangeBath = Convert.ToDouble(SplitComma(Promotion_OVS.BudgetChangeBath.ToString())),
                        BudgetRemarkBath = Promotion_OVS.BudgetRemarkBath,
                        GetDiscount = Promotion_OVS.GetDiscount,
                        GetPoint = Promotion_OVS.GetPoint,
                        PromotionConditions = Promotion_OVS.PromotionConditions,
                        MaketingRemark = Promotion_OVS.MaketingRemark,
                        CreateBy = _authService.GetClaim().UserId,
                        CreateDate = requestdate,
                        RequestStatus = BtnType,
                        RequestType = "OVS",
                        PromotionRef = Proref
                    };

                    var addOVS = await promotionRepo.InsertAsync(newOVS);

                    // Upload file.
                    string basePath = $"wwwroot/files/promotion/ovs/{(int)addOVS}";

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
                                CCId = (int)addOVS,
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
                                CCId = (int)addOVS,
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
                                CCId = (int)addOVS,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 3,
                                FilePath = $"{basePath}",
                                FileName = $"{FileLeft_3 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }

                    if (UploadFileRight_1 != null && FileRight_1 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileRight_1.FileName.Substring(UploadFileRight_1.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileRight_1 + "." + TypeFile}"))
                        {
                            await UploadFileRight_1.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = (int)addOVS,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{FileRight_1 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }

                    if (UploadFileRight_2 != null && FileRight_2 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileRight_2.FileName.Substring(UploadFileRight_2.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileRight_2 + "." + TypeFile}"))
                        {
                            await UploadFileRight_2.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = (int)addOVS,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 2,
                                FilePath = $"{basePath}",
                                FileName = $"{FileRight_2 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }

                    if (UploadFileRight_3 != null && FileRight_3 != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileRight_3.FileName.Substring(UploadFileRight_3.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{FileRight_3 + "." + TypeFile}"))
                        {
                            await UploadFileRight_3.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new PromotionAttachFileTable
                            {
                                CCId = (int)addOVS,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FileNo = 3,
                                FilePath = $"{basePath}",
                                FileName = $"{FileRight_3 + "." + TypeFile}",
                                FileLevel = 0
                            });
                        }
                    }

                    if (TypeOVS == "OVS")
                    {
                        TypeOVS = "OVS";
                    }else{
                        TypeOVS = "OVS Excep";
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

                    // update approve step
                    var currentRecord = await promotionRepo.GetAsync((int)addOVS);
                    currentRecord.CurrentApproveStep = 1;

                    await promotionRepo.UpdateAsync(currentRecord);

                    // check btn type draft
                    if(BtnType == 1){

                        // insert approve transaction
                        foreach (var item in approveFlow)
                        {
                            await approveTransRepo.InsertAsync(new PromotionApproveTransTable
                            {
                                Email = item.Email,
                                ApproveLevel = item.ApproveLevel,
                                ApproveMasterId = item.ApproveMasterId,
                                ApproveFlowId = item.Id,
                                CCId = (int)addOVS,
                                Position = item.Position,
                                Status = item.Status,
                                Name = item.Name,
                                LastName = item.LastName,
                            });
                        }

                        // update approve trans
                        var approveTransByCCId = await unitOfWork.Promotion.GetApproveTransByCCId((int)addOVS);

                        var approveTransLevel1 = approveTransByCCId.Where(x => x.ApproveLevel == 1).FirstOrDefault();
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
                        // Console.WriteLine(_authService.GetClaim().Email);
                        // send email level 1 to approve
                        if (Proref > 0 && Regex.IsMatch(prefixType, "Excep?"))
                        {
                            var refNumber = await promotionRepo.GetAsync((int)Proref);
                            var sendEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {reqNumber}",
                                $@"
                                    <b>เลขที่คำขอ : </b> {reqNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {Promotion_OVS.Pattern}<br/>
                                    <b>ชื่อลูกค้า : </b> {Promotion_OVS.CustomerName}<br/>
                                    <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={addOVS}&tid={approveTransLevel1.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                    <br/><b>เอกสารอ้างอิงเลขเดิม</b><br/>
                                    <b>เลขที่คำขอ : </b> {refNumber.RequestNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {refNumber.Pattern}<br/>
                                    <b>ลูกค้า/Area : </b> {refNumber.CustomerName}<br/>
                                    <b>Link อ้างอิง:</b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/{Proref}/Render'>คลิกที่นี่</a> <br/>
                                ",
                                new List<string> { approveFlow[0].Email },
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
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {reqNumber}",
                                $@"
                                    <b>เลขที่คำขอ : </b> {reqNumber}<br/>
                                    <b>ชื่อเรื่อง : </b> {Promotion_OVS.Pattern}<br/>
                                    <b>ชื่อลูกค้า : </b> {Promotion_OVS.CustomerName}<br/>
                                    <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={addOVS}&tid={approveTransLevel1.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                ",
                                new List<string> { approveFlow[0].Email },
                                new List<string> { },
                                _authService.GetClaim().Email
                            );

                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }
                        }
                    
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Add OVS Success. RequestNumber : " + reqNumber;
                    return Redirect("/Promotion");
                }

            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Promotion/AddOVS");
            }

        }

        public async Task<IActionResult> OnGetPromotionRefAsync(int IdRef)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var promotionOVSRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var ovs = await promotionOVSRepo.GetAsync(IdRef);

                    Promotion_OVS = ovs;

                    AttachFile = new List<PromotionAttachFileTable>();
                    var attachFile = await unitOfWork.Promotion.GetFileByCCIdAsync(IdRef, nameof(RequestTypeModel.OVS));

                    AttachFile = attachFile.ToList();
                    var listFiles = "";
                    // foreach (var item in AttachFile)
                    // {
                    //     // Console.WriteLine(item.FileName);
                    //     listFiles += "<li><a href='/Promotion/Index?handler=Download&id=" + item.Id + "' target='_blank'>" + item.FileName + "</a></li>";
                    // }
                    listFiles = "<a href='/Promotion/Export/OVS/" + IdRef + "?handler=PDF' target='_blank'>" + ovs.RequestNumber + ".pdf</a>";

                    unitOfWork.Complete();
                    return new JsonResult(new
                    {
                        typeOfProduct = Promotion_OVS.TypeOfProduct,
                        pattern = Promotion_OVS.Pattern,
                        customerName = Promotion_OVS.CustomerName,
                        typeOf = Promotion_OVS.TypeOf,
                        typeOfRemark = Promotion_OVS.TypeOfRemark,
                        customerGroup = Promotion_OVS.CustomerGroup,
                        customerGroupRemark = Promotion_OVS.CustomerGroupRemark,
                        paymentType = Promotion_OVS.PaymentType,
                        fromDate = Promotion_OVS.FromDate,
                        toDate = Promotion_OVS.ToDate,
                        typeFrom = Promotion_OVS.TypeFrom,
                        typeFromRemark = Promotion_OVS.TypeFromRemark,
                        objective = Promotion_OVS.Objective,
                        salesPresentBath = Promotion_OVS.SalesPresentBath,
                        salesForecastBath = Promotion_OVS.SalesForecastBath,
                        salesChangeBath = Promotion_OVS.SalesChangeBath,
                        salesRemarkBath = Promotion_OVS.SalesRemarkBath,
                        salesPresentQty = Promotion_OVS.SalesPresentQty,
                        salesForecastQty = Promotion_OVS.SalesForecastQty,
                        salesChangeQty = Promotion_OVS.SalesChangeQty,
                        salesRemarkQty = Promotion_OVS.SalesRemarkQty,
                        budgetPresent = Promotion_OVS.BudgetPresent,
                        budgetForecast = Promotion_OVS.BudgetForecast,
                        budgetChange = Promotion_OVS.BudgetChange,
                        budgetRemark = Promotion_OVS.BudgetRemark,
                        budgetPresentBath = Promotion_OVS.BudgetPresentBath,
                        budgetForecastBath = Promotion_OVS.BudgetForecastBath,
                        budgetChangeBath = Promotion_OVS.BudgetChangeBath,
                        budgetRemarkBath = Promotion_OVS.BudgetRemarkBath,
                        getDiscount = Promotion_OVS.GetDiscount,
                        getPoint = Promotion_OVS.GetPoint,
                        promotionConditions = Promotion_OVS.PromotionConditions,
                        listFile = listFiles
                    });
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
                var promotionTypeByProductRepo = new GenericRepository<PromotionByProductTable>(unitOfWork.Transaction);

                var promotionTypeByProdictAll = await promotionTypeByProductRepo.GetAllAsync();

                return promotionTypeByProdictAll
                    .Where(x => x.ByType == "OVS")
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.ByCode + " (" + x.ByName + ")",
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

        public async Task<List<SelectListItem>> GetPromotionTransMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionTransRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);

                var promotionTransAll = await promotionTransRepo.GetAllAsync();

                return promotionTransAll
                    .Where(x => x.RequestType == "OVS" && x.RequestStatus == 5)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.RequestNumber,
                    })
                    .ToList();
            }
        }

        private string SplitComma(string score)
        {
            string[] array = score.Split(',');
            string result = "";
            foreach (string value in array)
            {
                result += value;
            }
            return result;
        }
    }
}