using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
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
using Web.UI.Infrastructure.Constants.CreditControl;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels.CreditControl;
using Web.UI.Interfaces;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Web.UI.Pages.CreditControl
{
    public class AddDOMModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [BindProperty]
        public FormAddDOMViewModel FormAddDOMViewModel { get; set; } = new FormAddDOMViewModel();

        [BindProperty]
        public AddressTable Address1 { get; set; } = new AddressTable();

        [BindProperty]
        public AddressTable Address2 { get; set; } = new AddressTable();

        public List<CreditControlApproveTransTable> AllComment { get; set; } = new List<CreditControlApproveTransTable>();

        public string SaleName { get; set; } = "";

        // Master
        public List<CustomerTypeByProductTable> CustomerTypeByProductMaster { get; set; }
        public List<PaymentMethodTable> PaymentMethodMaster { get; set; }

        // Select List
        public List<SelectListItem> CustomerTypeSelectList { get; set; }
        public List<SelectListItem> ProvinceSelectList { get; set; }
        public List<SelectListItem> DistrictSelectList { get; set; }
        public List<SelectListItem> SubDistrictSelectList { get; set; }
        public List<SelectListItem> TermOfDeliverySelectList { get; set; }
        public List<SelectListItem> PaymentTermSelectList { get; set; }

        // Custom Binding
        public IFormFileCollection FileUpload { get; set; }

        [BindProperty]
        public List<int> CustomerTypeByProductCheckList { get; set; }

        [BindProperty]
        public List<int> PaymentMethodCheckList { get; set; }

        [BindProperty]
        public List<string> PaymentMethodCheckListText { get; set; }

        [BindProperty]
        public int SaveDraft { get; set; }

        [BindProperty]
        public bool FileCheck_1 { get; set; }
        [BindProperty]
        public bool FileCheck_2 { get; set; }
        [BindProperty]
        public bool FileCheck_3 { get; set; }
        [BindProperty]
        public bool FileCheck_4 { get; set; }
        [BindProperty]
        public bool FileCheck_5 { get; set; }
        [BindProperty]
        public bool FileCheck_6 { get; set; }
        [BindProperty]
        public bool FileCheck_7 { get; set; }
        [BindProperty]
        public bool FileCheck_8 { get; set; }
        [BindProperty]
        public bool FileCheck_9 { get; set; }
        [BindProperty]
        public bool FileCheck_10 { get; set; }
        [BindProperty]
        public bool FileCheck_11 { get; set; }
        [BindProperty]
        public bool FileCheck_12 { get; set; }
        [BindProperty]
        public bool FileCheck_13 { get; set; }
        [BindProperty]
        public bool FileCheck_14 { get; set; }

        // DI
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddDOMModel(
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

        private async Task LoadData()
        {
            FormAddDOMViewModel.RequestDate = DateTime.Now;

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var t = unitOfWork.Transaction;

                // get adata
                var user = await t.Connection.GetAsync<UserTable>(_authService.GetClaim().UserId, t);
                var employeeInfo = await unitOfWork.Employee.GetEmployeeByEmployeeId(user.EmployeeId);

                // assign to model
                CustomerTypeByProductMaster = t.Connection.GetAll<CustomerTypeByProductTable>(t).ToList();

                PaymentMethodMaster = t.Connection.GetAll<PaymentMethodTable>(t).ToList();

                CustomerTypeSelectList = t.Connection.GetAll<CustomerTypeTable>(t)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.TypeCode + " - " + x.Description
                    })
                    .ToList();

                ProvinceSelectList = t.Connection.GetAll<ProvinceTable>(t)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameInThai
                    })
                    .ToList();

                DistrictSelectList = t.Connection.GetAll<DistrictTable>(t)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameInThai
                    })
                    .ToList();

                TermOfDeliverySelectList = typeof(MethodOfPaymentModel).GetProperties()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Name,
                        Text = MethodOfPaymentModel.GetText(x.Name)
                    })
                    .ToList();

                PaymentTermSelectList = t.Connection.GetAll<PaymentTermTable>(t)
                    .Where(x => x.ByDOM == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.PaymentTermId + " (" + x.Description + ") " + x.DataAreaId.ToUpper()
                    })
                    .ToList();

                // set sale data
                if (employeeInfo != null)
                {
                    FormAddDOMViewModel.SaleEmployeeId = employeeInfo.EmployeeId;
                    SaleName = employeeInfo.Name + " " + employeeInfo.LastName;
                }

                unitOfWork.Complete();
            }
        }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (CustomerTypeByProductCheckList.Count == 0)
                {
                    ModelState.AddModelError("", "กรุณาเลือก ประเภทกลุ่มลูกค้าในระบบ (Type of Customer Group)");
                }

                if (!ModelState.IsValid)
                {
                    await LoadData();
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var t = unitOfWork.Transaction;

                    // Update number seq
                    var numberSeq = await t.Connection.GetAllAsync<NumberSeqTable>(t);
                    var numberSeqDOM = numberSeq.Where(x => x.SeqKey == nameof(NumberSeqModel.CreditControl_DOM)).FirstOrDefault();
                    numberSeqDOM.SeqValue += 1;
                    await t.Connection.UpdateAsync<NumberSeqTable>(numberSeqDOM, t);

                    string reqNumber = _creditControlService.GetLatestRequestNumberAsync(numberSeqDOM.SeqValue.ToString(), "D");

                    // add dom 
                    var newDom = new CreditControlDOMTable
                    {
                        RequestNumber = reqNumber,
                        CustomerCode = FormAddDOMViewModel.CustomerCode,
                        CompanyName = FormAddDOMViewModel.CompanyName,
                        RequestDate = FormAddDOMViewModel.RequestDate,
                        IsHeadQuarter = FormAddDOMViewModel.IsHeadQuarter,
                        Branch = FormAddDOMViewModel.Branch,
                        RequestType = RequestTypeModel.DOM,
                        TypeOfBusiness = FormAddDOMViewModel.TypeOfBusiness,
                        CreateBy = _authService.GetClaim().UserId,
                        CreateDate = DateTime.Now,
                        GuaranteeDOM_BGTotal = FormAddDOMViewModel.GuaranteeDOM_BGTotal,
                        CommentFromApproval = FormAddDOMViewModel.CommentFromApproval,
                        CommentFromSale = FormAddDOMViewModel.CommentFromSale,
                        CreditRating = FormAddDOMViewModel.CreditRating,
                        CreditLimited = FormAddDOMViewModel.CreditLimited,
                        DimensionNo = FormAddDOMViewModel.DimensionNo,
                        DiscountByConstant = FormAddDOMViewModel.DiscountByConstant,
                        DiscountByStep = FormAddDOMViewModel.DiscountByStep,
                        GuaranteeDOM_Check = FormAddDOMViewModel.GuaranteeDOM_Check,
                        GuaranteeDOM_ExpireDate = FormAddDOMViewModel.GuaranteeDOM_ExpireDate,
                        GuaranteeDOM_Other = FormAddDOMViewModel.GuaranteeDOM_Other,
                        GuaranteeDOM_IssueDate = FormAddDOMViewModel.GuaranteeDOM_IssueDate,
                        SaleEmployeeId = FormAddDOMViewModel.SaleEmployeeId,
                        TermOfDelivery = FormAddDOMViewModel.TermOfDelivery,
                        TermOfDelivery_Other = FormAddDOMViewModel.TermOfDelivery_Other,
                        TermOfPayment = FormAddDOMViewModel.TermOfPayment,
                        SaleZone = FormAddDOMViewModel.SaleZone,
                        RequestStatus = 1 // open
                    };

                    var addDom = await t.Connection.InsertAsync<CreditControlDOMTable>(newDom, t);

                    // add payment method trans
                    foreach (var item in PaymentMethodCheckList)
                    {
                        await t.Connection.InsertAsync<PaymentMethodTransTable>(new PaymentMethodTransTable
                        {
                            CCId = addDom,
                            PaymentMethodId = item,
                            Remark = PaymentMethodCheckListText[item - 1] ?? null
                        }, t);
                    }

                    // add check list file
                    if (FileCheck_1) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 1 }, t);
                    if (FileCheck_2) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 2 }, t);
                    if (FileCheck_3) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 3 }, t);
                    if (FileCheck_4) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 4 }, t);
                    if (FileCheck_5) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 5 }, t);
                    if (FileCheck_6) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 6 }, t);
                    if (FileCheck_7) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 7 }, t);
                    if (FileCheck_8) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 8 }, t);
                    if (FileCheck_9) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 9 }, t);
                    if (FileCheck_10) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 10 }, t);
                    if (FileCheck_11) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 11 }, t);
                    if (FileCheck_12) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 12 }, t);
                    if (FileCheck_13) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 13 }, t);
                    if (FileCheck_14) await t.Connection.InsertAsync<CreditControlCheckListFileTable>(new CreditControlCheckListFileTable { CCId = addDom, FileNo = 14 }, t);

                    //add customer type trans
                    if (CustomerTypeByProductCheckList.Count > 0)
                    {
                        foreach (var item in CustomerTypeByProductCheckList)
                        {
                            await t.Connection.InsertAsync<CustomerTypeByProductTransTable>(new CustomerTypeByProductTransTable
                            {
                                CCId = addDom,
                                CustomerCode = null,
                                CustomerByProductId = item,
                                CreateDate = DateTime.Now
                            }, t);
                        }
                    }

                    // get all comment
                    var allComment = await unitOfWork.CreditControl.GetApproveTransByCCId(addDom);
                    AllComment = allComment.OrderBy(x => x.ApproveLevel).ToList();

                    // add address
                    var newAddress1 = new AddressTable
                    {
                        CCId = addDom,
                        AddressType = nameof(AddressTypeModel.Address1_DOM),
                        AddressNo = Address1.AddressNo,
                        Moo = Address1.Moo,
                        Soi = Address1.Soi,
                        Street = Address1.Street,
                        Province = Address1.Province,
                        District = Address1.District,
                        SubDistrict = Address1.SubDistrict,
                        Email = Address1.Email,
                        Fax = Address1.Fax,
                        Tel = Address1.Tel,
                        ZipCode = Address1.ZipCode
                    };

                    var newAddress2 = new AddressTable
                    {
                        CCId = addDom,
                        AddressType = nameof(AddressTypeModel.Address2_DOM),
                        AddressNo = Address2.AddressNo,
                        Moo = Address2.Moo,
                        Soi = Address2.Soi,
                        Street = Address2.Street,
                        Province = Address2.Province,
                        District = Address2.District,
                        SubDistrict = Address2.SubDistrict,
                        Email = Address2.Email,
                        Fax = Address2.Fax,
                        Tel = Address2.Tel,
                        ZipCode = Address2.ZipCode
                    };

                    await t.Connection.InsertAsync<AddressTable>(newAddress1, t);
                    await t.Connection.InsertAsync<AddressTable>(newAddress2, t);

                    // get approve mapping 
                    var approveMapping = await unitOfWork.CreditControl.GetApproveGroupId(nameof(CreditControlTypeModel.DOM), _authService.GetClaim().UserId);
                    if (approveMapping == null)
                    {
                        throw new Exception("Approve mapping not match!");
                    }

                    // get all approve flow 
                    var approveFlowAll = await t.Connection.GetAllAsync<ApproveFlowTable>(t);

                    var approveFlow = approveFlowAll.Where(x =>
                        x.ApproveMasterId == approveMapping.ApproveMasterId &&
                        x.IsActive == 1)
                        .OrderBy(x => x.ApproveLevel)
                        .ToList();

                    if (approveFlow.Count == 0)
                    {
                        throw new Exception("Approve flow not found.");
                    }

                    // insert approve transaction
                    foreach (var item in approveFlow)
                    {
                        await t.Connection.InsertAsync<CreditControlApproveTransTable>(new CreditControlApproveTransTable
                        {
                            Email = item.Email,
                            ApproveLevel = item.ApproveLevel,
                            ApproveMasterId = item.ApproveMasterId,
                            ApproveFlowId = item.Id,
                            CCId = (int)addDom,
                        }, t);
                    }

                    // update approve step
                    var currentRecord = await t.Connection.GetAsync<CreditControlDOMTable>(addDom, t);
                    currentRecord.CurrentApproveStep = 1;

                    if (SaveDraft == 1)
                    {
                        currentRecord.RequestStatus = (int)StatusModel.RequestStatus.Draft; // draft
                    }

                    await t.Connection.UpdateAsync<CreditControlDOMTable>(currentRecord, t);

                    // update approve trans
                    var approveTransByCCId = await unitOfWork.CreditControl.GetApproveTransByCCId(addDom);

                    var approveTransLevel1 = approveTransByCCId.Where(x => x.ApproveLevel == 1).FirstOrDefault();

                    var approveTrans = await t.Connection.GetAsync<CreditControlApproveTransTable>(approveTransLevel1.Id, t);

                    approveTrans.SendEmailDate = DateTime.Now;

                    await t.Connection.UpdateAsync<CreditControlApproveTransTable>(approveTrans, t);

                    // Upload file.
                    string basePath = $"wwwroot/files/credit_control/dom/{(int)addDom}";

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    var filePath = Path.GetTempFileName();

                    if (FileUpload.Count == 0)
                    {
                        SaveDraft = 1;
                    }

                    foreach (var file in FileUpload)
                    {
                        using (var stream = System.IO.File.Create($"{basePath}/{file.FileName}"))
                        {
                            await file.CopyToAsync(stream);

                            await t.Connection.InsertAsync<CreditControlAttachFileTable>(new CreditControlAttachFileTable
                            {
                                CCId = (int)addDom,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FilePath = $"{basePath}",
                                FileName = $"{file.FileName}"
                            }, t);
                        }
                    }

                    // generate nonce
                    var nonceKey = Guid.NewGuid().ToString();

                    await t.Connection.InsertAsync<NonceTable>(new NonceTable
                    {
                        NonceKey = nonceKey,
                        CreateDate = DateTime.Now,
                        ExpireDate = DateTime.Now.AddDays(7),
                        IsUsed = 0
                    }, t);

                    // get sender
                    string sender = "it_ea@deestone.com";

                    var user = await t.Connection.GetAsync<UserTable>(_authService.GetClaim().UserId, t);
                    if (user != null)
                    {
                        if (user.Email != null && user.Email != "")
                            sender = user.Email;
                    }

                    if (SaveDraft != 1) // 1 = draft
                    {
                        // send email level 1 to approve
                        var sendEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {reqNumber}",
                                $@"
                                <b>เลขที่คำขอ:</b> {reqNumber}<br/>
                                <b>ชื่อลูกค้า:</b> {FormAddDOMViewModel.CompanyName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveDOM?id={addDom}&tid={approveTransLevel1.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                            ",
                                new List<string> { approveFlow[0].Email },
                                new List<string> { },
                                sender
                            );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }
                    }

                    // commit
                    unitOfWork.Complete();

                    AlertSuccess = "Add DOM Success.";
                    return Redirect("/CreditControl/AddDOM");
                }
            }
            catch (Exception ex)
            {
                Address1.Province = null;
                Address2.Province = null;

                await LoadData();

                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        // Custom Handler

        public async Task<JsonResult> OnPostEmployeeGridAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var employeeRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);

                var field = new
                {
                    employeeId = "EmployeeId",
                    name = "Name",
                    lastName = "LastName",
                    nameEng = "NameEng",
                    company = "Company",
                    positionName = "PositionName",
                    divisionName = "DivisionName",
                    departmentName = "DepartmentName",
                    employeeIdOld = "EmployeeIdOld",
                    email = "Email"
                };

                var filter = _datatableService.Filter(Request, field);

                var employeeAll = await unitOfWork.Transaction.Connection.QueryAsync<EmployeeTable>(@"
                    SELECT TOP 500
                        EmployeeId,
                        Name,
                        LastName,
                        NameEng,
                        Company,
                        PositionName,
                        DivisionName,
                        DepartmentName,
                        EmployeeIdOld,
                        Email
                    FROM TB_Employee
                    WHERE " + filter + @" 
                    AND Status <> 9
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatableService.Format(Request, employeeAll.ToList()));
            }
        }

        public async Task<JsonResult> OnGetDistrictByProvinceAsync(int provinceId)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var districtRepo = new GenericRepository<DistrictTable>(unitOfWork.Transaction);

                var districtAll = await districtRepo.GetAllAsync();

                unitOfWork.Complete();

                return new JsonResult(districtAll.Where(x => x.ProvinceId == provinceId).ToList());
            }
        }

        public async Task<JsonResult> OnGetSubDistrictByProvinceAsync(int districtId)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var subDistrictRepo = new GenericRepository<SubDistrictTable>(unitOfWork.Transaction);

                var subDistrictAll = await subDistrictRepo.GetAllAsync();

                unitOfWork.Complete();

                return new JsonResult(subDistrictAll.Where(x => x.DistrictId == districtId).ToList());
            }
        }

        public async Task<IActionResult> OnGetPaymentTypeAsync(int termId)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var paymentTermRepo = new GenericRepository<PaymentTermTable>(unitOfWork.Transaction);

                var paymentTerm = await paymentTermRepo.GetAsync(termId);

                if (paymentTerm == null)
                {
                    return new JsonResult(new { UploadType = 0 });
                }

                if (paymentTerm.PaymentTermId.Equals("CASH") || paymentTerm.PaymentTermId.Equals("CASH_ND"))
                {
                    return new JsonResult(new { UploadType = 1 });
                }

                if (paymentTerm.PaymentTermId.Equals("D15") ||
                    paymentTerm.PaymentTermId.Equals("D30") ||
                    paymentTerm.PaymentTermId.Equals("D30/ND") ||
                    paymentTerm.PaymentTermId.Equals("D45") ||
                    paymentTerm.PaymentTermId.Equals("D60") ||
                    paymentTerm.PaymentTermId.Equals("D60/ND") ||
                    paymentTerm.PaymentTermId.Equals("D90")
                    )
                {
                    return new JsonResult(new { UploadType = 2 });
                }

                return new JsonResult(new { UploadType = 0 });
            }
        }

        public async Task<JsonResult> OnGetZipCodeBySubDistrictAsync(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var t = unitOfWork.Transaction;

                var subDistrict = await t.Connection.GetAllAsync<SubDistrictTable>(t);


                var zipCode = subDistrict.Where(x => x.Id == id).FirstOrDefault();

                unitOfWork.Complete();

                return new JsonResult(new { ZipCode = zipCode.ZipCode });
            }
        }

        // End Custom Handler
    }
}