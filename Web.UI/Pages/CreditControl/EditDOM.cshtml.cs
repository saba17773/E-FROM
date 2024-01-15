using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl
{
    public class EditDOMModel : PageModel
    {

        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public CreditControlDOMTable CreditControl_DOM { get; set; }

        [BindProperty]
        public AddressTable Address1 { get; set; }

        [BindProperty]
        public AddressTable Address2 { get; set; }

        public List<AllCommentsApprovalViewModel> AllComment { get; set; }

        public string SaleName { get; set; }

        [BindProperty]
        public int SendApprove { get; set; }

        public List<PaymentMethodTable> PaymentMethodList { get; set; }

        [BindProperty]
        public List<int> PaymentMethod { get; set; }

        [BindProperty]
        public List<string> PaymentMethodText { get; set; }
        public List<PaymentMethodTransTable> PaymentMethodTransSelected { get; set; }

        // Master
        public List<SelectListItem> CustomerTypeMaster { get; set; }
        public List<SelectListItem> CustomerTypeByProductMaster { get; set; }

        public List<SelectListItem> ProvinceMaster { get; set; }
        public List<SelectListItem> DistrictMaster { get; set; }
        public List<SelectListItem> SubDistrictMaster { get; set; }

        //public List<SelectListItem> PaymentMethodMaster { get; set; }
        public List<SelectListItem> PaymentTermMaster { get; set; }
        public List<SelectListItem> TermOfDeliveryMaster { get; set; }

        public bool EditCreditLimit { get; set; }

        public List<CustomerTypeTransViewModel> CustomerTypeTrans { get; set; }

        [BindProperty]
        public List<int> TypeOfCustomer { get; set; }

        public IEnumerable<CreditControlCheckListFileTable> CheckListFile { get; set; }

        // file upload
        public IFormFileCollection FileUpload { get; set; }
        public int FileUploadType { get; set; }
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

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public EditDOMModel(
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
            CreditControl_DOM = new CreditControlDOMTable();
            Address1 = new AddressTable();
            Address2 = new AddressTable();
            AllComment = new List<AllCommentsApprovalViewModel>();

            //CreditControl_DOM.RequestDate = DateTime.Now.Date;
            CustomerTypeMaster = await GetCustomerTypeMasterAsync();
            CustomerTypeByProductMaster = await GetCustomerTypeByProductMasterAsync();
            ProvinceMaster = await GetProvinceMasterAsync();
            DistrictMaster = await GetDistrictMasterAsync();
            SubDistrictMaster = await GetSubDistrictMasterAsync();
            //PaymentMethodMaster = await GetPaymentMethodMasterAsync();
            PaymentTermMaster = await GetPaymentTermMasterAsync();
            TermOfDeliveryMaster = GetTermOfDeliveryMaster();

            PaymentMethodList = await GetPaymentMethodListAsync();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var creditControlDOMRepo = new GenericRepository<CreditControlDOMTable>(unitOfWork.Transaction);
                var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                var customerTypeTrans = new GenericRepository<CustomerTypeByProductTransTable>(unitOfWork.Transaction);
                var customerByProduct = new GenericRepository<CustomerTypeByProductTable>(unitOfWork.Transaction);
                var checkListFileTable = new GenericRepository<CreditControlCheckListFileTable>(unitOfWork.Transaction);

                var dom = await creditControlDOMRepo.GetAsync(id);
                var approveTrans = await unitOfWork.CreditControl.GetApproveTransByCCId(id);

                CheckListFile = await unitOfWork.CreditControl.GetCheckListFileByCCIdAsync(dom.Id);

                CreditControl_DOM = dom;

                if (CreditControl_DOM.RequestStatus == RequestStatusModel.Complete)
                {
                    throw new Exception("รายการถูกดำเนินการเสร็จสิ้นไปแล้ว ไม่สามารถแก้ไขได้");
                }
                else if (CreditControl_DOM.RequestStatus == RequestStatusModel.Cancel)
                {
                    throw new Exception("รายการถูกยกเลิกไปแล้ว ไม่สามารถแก้ไขได้");
                }
                else if (CreditControl_DOM.RequestStatus == RequestStatusModel.WaitingForApprove)
                {
                    throw new Exception("รายการอยู่ระหว่างการอนุมัติ ไม่สามารถแก้ไขได้");
                }

                //CreditControl_DOM.RequestDate = Convert.ToDateTime(dom.RequestDate.ToString("d", new CultureInfo("en-US")));

                Address1 = await unitOfWork.CreditControl.GetAddressByCCIdAsync(id, nameof(AddressTypeModel.Address1_DOM));
                Address2 = await unitOfWork.CreditControl.GetAddressByCCIdAsync(id, nameof(AddressTypeModel.Address2_DOM));

                //AllComment = allComment.OrderBy(x => x.ApproveLevel).ToList();

                var allCommentsApproval = new List<AllCommentsApprovalViewModel>();

                foreach (var item in approveTrans.OrderBy(x => x.ApproveLevel).ToList())
                {
                    var approveFlowName = await approveFlowRepo.GetAsync(item.ApproveFlowId);

                    if (approveFlowName != null)
                    {
                        allCommentsApproval.Add(new AllCommentsApprovalViewModel
                        {
                            Name = approveFlowName.Name,
                            Email = item.Email,
                            Remark = item.Remark
                        });
                    }
                }

                AllComment = allCommentsApproval;

                var employee = await unitOfWork.Employee.GetEmployeeByEmployeeId(dom.SaleEmployeeId);
                if (employee != null)
                {
                    SaleName = employee.Name + " " + employee.LastName;
                }
                else
                {
                    SaleName = "";
                }

                var approveFlow = await unitOfWork.CreditControl.GetApproveFlowByCCIdAsync(dom.Id);
                var creditLimitEnable = approveFlow.Where(x => x.EditCreditLimit == 1);

                if (creditLimitEnable.FirstOrDefault() != null && creditLimitEnable.Any(x => x.Email == _authService.GetClaim().Email))
                {
                    EditCreditLimit = true;
                } 
                else
                {
                    EditCreditLimit = false;
                }

                var _customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(id);
                CustomerTypeTrans = _customerTypeTrans.ToList();

                var _paymentMEthodSelected = await unitOfWork.CreditControl.GetPaymentMethodTransByCCIdAsync(id);
                PaymentMethodTransSelected = _paymentMEthodSelected.ToList();

                unitOfWork.Complete();
            }

        }

        private async Task<List<PaymentMethodTable>> GetPaymentMethodListAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var paymethodTable = new GenericRepository<PaymentMethodTable>(unitOfWork.Transaction);

                var paymethodList = await paymethodTable.GetAllAsync();
                return paymethodList.ToList();
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
                return Redirect("/CreditControl");
            }
            
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await InitialDataAsync(id);

                    return Redirect($@"/CreditControl/{id}/EditDOM");
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var creditControlRepo = new GenericRepository<CreditControlDOMTable>(unitOfWork.Transaction);
                    var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<CreditControlAttachFileTable>(unitOfWork.Transaction);
                    var numberSeqRepo = new GenericRepository<NumberSeqTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var customerTypeTransTable = new GenericRepository<CustomerTypeByProductTransTable>(unitOfWork.Transaction);
                    var paymentMethodTransTable = new GenericRepository<PaymentMethodTransTable>(unitOfWork.Transaction);
                    var checkListFileTable = new GenericRepository<CreditControlCheckListFileTable>(unitOfWork.Transaction);

                    // update TB_CreditControl
                    var creditControl = await creditControlRepo.GetAsync(id);

                    //creditControl = CreditControl_DOM;

                    creditControl.CompanyName = CreditControl_DOM.CompanyName;
                    creditControl.RequestDate = CreditControl_DOM.RequestDate;
                    creditControl.IsHeadQuarter = CreditControl_DOM.IsHeadQuarter;
                    creditControl.Branch = CreditControl_DOM.Branch;
                    creditControl.RequestType = RequestTypeModel.DOM;
                    creditControl.TypeOfBusiness = CreditControl_DOM.TypeOfBusiness;
                    creditControl.TypeOfProduct = CreditControl_DOM.TypeOfProduct;
                    creditControl.CreateBy = _authService.GetClaim().UserId;
                    creditControl.CreateDate = DateTime.Now;
                    creditControl.GuaranteeDOM_BGTotal = CreditControl_DOM.GuaranteeDOM_BGTotal;
                    creditControl.CommentFromApproval = CreditControl_DOM.CommentFromApproval;
                    creditControl.CommentFromSale = CreditControl_DOM.CommentFromSale;
                    creditControl.CreditRating = CreditControl_DOM.CreditRating;
                    creditControl.CreditLimited = CreditControl_DOM.CreditLimited;
                    creditControl.DimensionNo = CreditControl_DOM.DimensionNo;
                    creditControl.DiscountByConstant = CreditControl_DOM.DiscountByConstant;
                    creditControl.DiscountByStep = CreditControl_DOM.DiscountByStep;
                    creditControl.GuaranteeDOM_Check = CreditControl_DOM.GuaranteeDOM_Check;
                    creditControl.GuaranteeDOM_ExpireDate = CreditControl_DOM.GuaranteeDOM_ExpireDate;
                    creditControl.GuaranteeDOM_Other = CreditControl_DOM.GuaranteeDOM_Other;
                    creditControl.GuaranteeDOM_IssueDate = CreditControl_DOM.GuaranteeDOM_IssueDate;
                    creditControl.TermOfPayment = CreditControl_DOM.TermOfPayment;
                    creditControl.SaleEmployeeId = CreditControl_DOM.SaleEmployeeId;
                    creditControl.TermOfDelivery = CreditControl_DOM.TermOfDelivery;
                    creditControl.TermOfDelivery_Other = CreditControl_DOM.TermOfDelivery_Other;
                    creditControl.SaleZone = CreditControl_DOM.SaleZone;
                    creditControl.PaymentMethod = CreditControl_DOM.PaymentMethod;

                    await creditControlRepo.UpdateAsync(creditControl);

                    // delete old customer type trans
                    await unitOfWork.Transaction.Connection.ExecuteAsync(@"
                        DELETE FROM TB_CustomerTypeByProductTrans WHERE CCId = @CCId
                    ", new { @CCId = creditControl.Id }, unitOfWork.Transaction);

                    // delete old payment method trans
                    await unitOfWork.Transaction.Connection.ExecuteAsync(@"
                        DELETE FROM TB_PaymentMethodTrans WHERE CCId = @CCId
                    ", new { @CCId = creditControl.Id }, unitOfWork.Transaction);

                    // delete checklist
                    await unitOfWork.Transaction.Connection.ExecuteAsync(@"
                        DELETE FROM TB_CreditControlCheckListFile WHERE CCId = @CCId
                    ", new { @CCId = creditControl.Id }, unitOfWork.Transaction);

                    // add check list file
                    if (FileCheck_1) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 1 });
                    if (FileCheck_2) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 2 });
                    if (FileCheck_3) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 3 });
                    if (FileCheck_4) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 4 });
                    if (FileCheck_5) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 5 });
                    if (FileCheck_6) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 6 });
                    if (FileCheck_7) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 7 });
                    if (FileCheck_8) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 8 });
                    if (FileCheck_9) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 9 });
                    if (FileCheck_10) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 10 });
                    if (FileCheck_11) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 11 });
                    if (FileCheck_12) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 12 });
                    if (FileCheck_13) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 13 });
                    if (FileCheck_14) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = creditControl.Id, FileNo = 14 });

                    // add payment method trans
                    foreach (var item in PaymentMethod)
                    {
                        await paymentMethodTransTable.InsertAsync(new PaymentMethodTransTable
                        {
                            CCId = creditControl.Id,
                            PaymentMethodId = item,
                            Remark = PaymentMethodText[item - 1] ?? null
                        });
                    }

                    // add customer type trans
                    if (TypeOfCustomer.Count > 0)
                    {
                        foreach (var item in TypeOfCustomer)
                        {
                            await customerTypeTransTable.InsertAsync(new CustomerTypeByProductTransTable
                            {
                                CCId = (int)creditControl.Id,
                                CustomerCode = null,
                                CustomerByProductId = item,
                                CreateDate = DateTime.Now
                            });
                        }
                    }

                    // Update TB_Address
                    var address1 = await unitOfWork.CreditControl.GetAddressByCCIdAsync(creditControl.Id, nameof(AddressTypeModel.Address1_DOM));
                    var address2 = await unitOfWork.CreditControl.GetAddressByCCIdAsync(creditControl.Id, nameof(AddressTypeModel.Address2_DOM));

                    //address1 = Address1;
                    address1.AddressType = nameof(AddressTypeModel.Address1_DOM);
                    address1.AddressNo = Address1.AddressNo;
                    address1.Moo = Address1.Moo;
                    address1.Soi = Address1.Soi;
                    address1.Street = Address1.Street;
                    address1.Province = Address1.Province;
                    address1.District = Address1.District;
                    address1.SubDistrict = Address1.SubDistrict;
                    address1.Email = Address1.Email;
                    address1.Fax = Address1.Fax;
                    address1.Tel = Address1.Tel;
                    address1.ZipCode = Address1.ZipCode;

                    //address2 = Address2;
                    address2.AddressType = nameof(AddressTypeModel.Address2_DOM);
                    address2.AddressNo = Address2.AddressNo;
                    address2.Moo = Address2.Moo;
                    address2.Soi = Address2.Soi;
                    address2.Street = Address2.Street;
                    address2.Province = Address2.Province;
                    address2.District = Address2.District;
                    address2.SubDistrict = Address2.SubDistrict;
                    address2.Email = Address2.Email;
                    address2.Fax = Address2.Fax;
                    address2.Tel = Address2.Tel;
                    address2.ZipCode = Address2.ZipCode;

                    await addressRepo.UpdateAsync(address1);
                    await addressRepo.UpdateAsync(address2);

                    // upload file
                    string basePath = $"wwwroot/files/credit_control/dom/{id}";

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    else
                    {
                        System.IO.Directory.Delete(basePath);
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    var filePath = Path.GetTempFileName();

                    // delete file path in database
                    await unitOfWork.Transaction.Connection.ExecuteAsync(@" DELETE FROM TB_CreditControlAttachFile WHERE CCId = @CCId", new { @CCId = creditControl.Id }, unitOfWork.Transaction);

                    foreach (var file in FileUpload)
                    {
                        using (var stream = System.IO.File.Create($"{basePath}/{file.FileName}"))
                        {
                            await file.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new CreditControlAttachFileTable
                            {
                                CCId = creditControl.Id,
                                CCType = nameof(CreditControlTypeModel.DOM),
                                FilePath = $"{basePath}",
                                FileName = $"{file.FileName}"
                            });
                        }
                    }

                    // resend email if status is = reject
                    if (creditControl.RequestStatus == RequestStatusModel.Reject)
                    {
                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = DateTime.Now,
                            ExpireDate = DateTime.Now.AddDays(7),
                            IsUsed = 0
                        });

                        var currentApproveTransByCCId = await unitOfWork.CreditControl.GetApproveTransByCCId(id);

                        var currentApproveTrans = currentApproveTransByCCId
                            .OrderBy(x => x.ApproveLevel)
                            .Where(x => x.IsDone == 0)
                            .FirstOrDefault();

                        if (currentApproveTrans != null)
                        {
                            // get editor email
                            var editorEmail = await userRepo.GetAsync((int)creditControl.CreateBy);

                            // send email level 1 to approve
                            var sendEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                                $@"
                                <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveDOM?id={id}&tid={currentApproveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                            ",
                                new List<string> { currentApproveTrans.Email },
                                new List<string> { },
                                editorEmail.Email
                            );

                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }
                        }
                    }

                    if (SendApprove == 1) // 1 = send approve
                    {
                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = DateTime.Now,
                            ExpireDate = DateTime.Now.AddDays(7),
                            IsUsed = 0
                        });

                        // get approve flow email
                        var currentApproveTransByCCId = await unitOfWork.CreditControl.GetApproveTransByCCId(id);

                        var currentApproveTrans = currentApproveTransByCCId
                            .OrderBy(x => x.ApproveLevel)
                            .Where(x => x.IsDone == 0)
                            .FirstOrDefault();

                        // send email level 1 to approve
                        var sendEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                                $@"
                                <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveDOM?id={id}&tid={currentApproveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                            ",
                                new List<string> { currentApproveTrans.Email },
                                new List<string> { },
                                "it_ea@deestone.com"
                            );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }
                    }

                    unitOfWork.Complete();


                    AlertSuccess = "Edit Success.";

                    if (creditControl.RequestStatus == RequestStatusModel.Reject)
                    {
                        
                        return Redirect($@"/CreditControl");
                    }
                    else
                    {
                        return Redirect($@"/CreditControl/{id}/EditDOM");
                    }

                    
                }

            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($@"/CreditControl/{id}/EditDOM");
            }
        }

        private async Task<List<SelectListItem>> GetPaymentTermMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var paymentTermRepo = new GenericRepository<PaymentTermTable>(unitOfWork.Transaction);

                var paymentTermAll = await paymentTermRepo.GetAllAsync();

                unitOfWork.Complete();

                return paymentTermAll
                   .Select(x => new SelectListItem
                   {
                       Value = x.Id.ToString(),
                       Text = x.PaymentTermId + " (" + x.Description + ") " + x.DataAreaId.ToUpper()
                   })
                   .ToList();
            }
        }

        private async Task<List<SelectListItem>> GetPaymentMethodMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var paymentMethodRepo = new GenericRepository<PaymentMethodTable>(unitOfWork.Transaction);

                var paymentMethodAll = await paymentMethodRepo.GetAllAsync();

                unitOfWork.Complete();

                return paymentMethodAll
                   .Select(x => new SelectListItem
                   {
                       Value = x.Id.ToString(),
                       Text = x.MethodOfPayment + " (" + x.MethodName + ") " + x.DataAreaId.ToUpper()
                   })
                   .ToList();
            }
        }

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
                    AND DepartmentCode IN (
                        1432,
                        1433,
                        1434,
                        1435,
                        1436,
                        1437,
                        1438,
                        1874,
                        1875,
                        1882,
                        2132)
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

        private async Task<List<SelectListItem>> GetDistrictMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var districtRepo = new GenericRepository<DistrictTable>(unitOfWork.Transaction);

                var districtAll = await districtRepo.GetAllAsync();

                return districtAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameInThai
                    })
                    .ToList();
            }
        }

        private async Task<List<SelectListItem>> GetSubDistrictMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var subDistrictRepo = new GenericRepository<SubDistrictTable>(unitOfWork.Transaction);

                var subDistrictAll = await subDistrictRepo.GetAllAsync();

                return subDistrictAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameInThai
                    })
                    .ToList();
            }
        }

        private async Task<List<SelectListItem>> GetProvinceMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var provinceRepo = new GenericRepository<ProvinceTable>(unitOfWork.Transaction);

                var provinceAll = await provinceRepo.GetAllAsync();

                return provinceAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.NameInThai
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetCustomerTypeMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var customerTypeRepo = new GenericRepository<CustomerTypeTable>(unitOfWork.Transaction);

                var customerTypeAll = await customerTypeRepo.GetAllAsync();

                return customerTypeAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.TypeCode + " - " + x.Description
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetCustomerTypeByProductMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var customerTypeByProductRepo = new GenericRepository<CustomerTypeByProductTable>(unitOfWork.Transaction);

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

        public List<SelectListItem> GetTermOfDeliveryMaster()
        {
            var lists = typeof(MethodOfPaymentModel).GetProperties().Select(x => new SelectListItem
            {
                Value = x.Name,
                Text = MethodOfPaymentModel.GetText(x.Name)
            }).ToList();

            return lists;
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
    }
}