using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.CreditControl;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Infrastructure.ViewModels.CreditControl;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl
{
    public class ApproveDOMModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public ViewDOMViewModel ViewDOMData { get; set; }

        [BindProperty]
        public FormCreditControlInfo_DOM FormCreditControl { get; set; }

        [BindProperty]
        public CreditControlDOMTable CreditControl_DOM { get; set; }

        public AddressTable Address1 { get; set; }

        public AddressTable Address2 { get; set; }

        [BindProperty]
        public string ApproveRemark { get; set; }

        [BindProperty]
        [Required]
        public int ApproveResult { get; set; }

        public string SaleName { get; set; }

        public List<CustomerTypeTransViewModel> CustomerTypeTrans { get; set; }
        public List<CustomerTypeByProductTable> CustomerTypeByProduct { get; set; }

        public List<PaymentMethodTable> PaymentMethodList { get; set; }

        [BindProperty]
        public List<int> PaymentMethod { get; set; }

        [BindProperty]
        public List<string> PaymentMethodText { get; set; }
        public List<PaymentMethodTransTable> PaymentMethodTransSelected { get; set; }

        // Master
        public List<SelectListItem> CustomerTypeMaster { get; set; }
        public List<SelectListItem> CustomerTypeByProductMaster { get; set; }

        public List<SelectListItem> PaymentMethodMaster { get; set; }
        public List<SelectListItem> PaymentTermMaster { get; set; }
        public List<SelectListItem> TermOfDeliveryMaster { get; set; }

        public List<CreditControlAttachFileTable> AttachFile { get; set; }

        public bool EditCreditLimit { get; set; }

        public List<CreditControl_ApproveRemarkModel> HistoryRemark { get; set; }

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

        private async Task InitialDataAsync(int id, int tid, string nonce)
        {
            CreditControl_DOM = new CreditControlDOMTable();
            Address1 = new AddressTable();
            Address2 = new AddressTable();

            FormCreditControl = new FormCreditControlInfo_DOM();

            AttachFile = new List<CreditControlAttachFileTable>();

            CreditControl_DOM.RequestDate = DateTime.Now.Date;
            CustomerTypeMaster = await GetCustomerTypeMasterAsync();
            CustomerTypeByProductMaster = await GetCustomerTypeByProductMasterAsync();
            PaymentMethodMaster = await GetPaymentMethodMasterAsync();
            PaymentTermMaster = await GetPaymentTermMasterAsync();
            TermOfDeliveryMaster = GetTermOfDeliveryMaster();

            CustomerTypeByProduct = await GetCustomerTypeByProductAsync();

            PaymentMethodList = await GetPaymentMethodListAsync();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
                var creditControlRepo = new GenericRepository<CreditControlDOMTable>(unitOfWork.Transaction);
                var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
                var typeOfBusinessRepo = new GenericRepository<CustomerTypeTable>(unitOfWork.Transaction);
                var provinceRepo = new GenericRepository<ProvinceTable>(unitOfWork.Transaction);
                var districtRepo = new GenericRepository<DistrictTable>(unitOfWork.Transaction);
                var subDistrictRepo = new GenericRepository<SubDistrictTable>(unitOfWork.Transaction);
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                ViewDOMData = await unitOfWork.CreditControl.GetDOMDataByCCIdAsync(id);

                var _nonce = await unitOfWork.Nonce.GetNonceByKey(nonce);

                if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
                {
                    throw new Exception("รายการนี้ถูกดำเนินการไปแล้ว");
                }

                var creditControl = await creditControlRepo.GetAsync(id);
                var approveTrans = await approveTransRepo.GetAsync(tid);

                CreditControl_DOM = creditControl;

                var attachFile = await unitOfWork.CreditControl.GetFileByCCIdAsync(id, nameof(RequestTypeModel.DOM));

                AttachFile = attachFile.ToList();

                // list customer type 
                var customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(creditControl.Id);
                CustomerTypeTrans = customerTypeTrans.ToList();

                // check payment method
                var _paymentMEthodSelected = await unitOfWork.CreditControl.GetPaymentMethodTransByCCIdAsync(id);
                PaymentMethodTransSelected = _paymentMEthodSelected.ToList();


                // check can edit credit limit
                var _approveFlow = await unitOfWork.CreditControl.GetApproveFlowByCCIdAsync(creditControl.Id);
                var _creditLimitEnable = _approveFlow.Where(x => x.EditCreditLimit == 1);

                if (_creditLimitEnable.FirstOrDefault() != null && _creditLimitEnable.Any(x => x.Email == approveTrans.Email))
                {
                    EditCreditLimit = true;
                }
                else
                {
                    EditCreditLimit = false;
                }

                var _approveTrans = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
                var approveTransByLevel = _approveTrans.OrderBy(x => x.ApproveLevel).ToList();

                List<CreditControl_ApproveRemarkModel> approveRemark = new List<CreditControl_ApproveRemarkModel>();

                foreach (var item in approveTransByLevel)
                {
                    var approveName = await approveFlowRepo.GetAsync(item.ApproveFlowId);
                    approveRemark.Add(new CreditControl_ApproveRemarkModel
                    {
                        Remark = item.Remark,
                        Name = approveName?.Name ?? ""
                    });
                }

                HistoryRemark = approveRemark;

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

        private async Task<List<CustomerTypeByProductTable>> GetCustomerTypeByProductAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var customerTypeByProductRepo = new GenericRepository<CustomerTypeByProductTable>(unitOfWork.Transaction);

                var customerTypeByProductAll = await customerTypeByProductRepo.GetAllAsync();
                return customerTypeByProductAll.ToList();
            }
        }

        public async Task<IActionResult> OnGet(int id, int tid, string nonce)
        {
            try
            {
                await InitialDataAsync(id, tid, nonce);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Result");
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

        public async Task<IActionResult> OnPostAsync(int id, int tid, string nonce)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะ อนุมัติ หรือ ไม่อนุมัติ";
                    return Redirect($"/Result");
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var t = unitOfWork.Transaction;

                    // check nonce
                    var _nonce = await unitOfWork.Nonce.GetNonceByKey(nonce);

                    if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
                    {
                        throw new Exception("Nonce expired.");
                    }

                    _nonce.IsUsed = 1;

                    var approveTrans = await t.Connection.GetAsync<CreditControlApproveTransTable>(tid, t);
                    var creditControl = await t.Connection.GetAsync<CreditControlDOMTable>(id, t);
                    var approveTransAll = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
                    var user = await t.Connection.GetAsync<UserTable>((int)creditControl.CreateBy, t);

                    creditControl.CreditLimited = FormCreditControl.CreditLimited;
                    creditControl.CreditRating = FormCreditControl.CreditRating;
                    creditControl.GuaranteeDOM_Check = FormCreditControl.GuaranteeDOM_Check;
                    creditControl.GuaranteeDOM_ExpireDate = FormCreditControl.GuaranteeDOM_ExpireDate;
                    creditControl.GuaranteeDOM_Other = FormCreditControl.GuaranteeDOM_Other;
                    creditControl.GuaranteeDOM_IssueDate = FormCreditControl.GuaranteeDOM_IssueDate;
                    creditControl.TermOfPayment = FormCreditControl.TermOfPayment;
                    creditControl.GuaranteeDOM_BGTotal = FormCreditControl.GuaranteeDOM_BGTotal;

                    creditControl.IsHeadQuarter = creditControl.IsHeadQuarter;
                    creditControl.CompanyName = creditControl.CompanyName;
                    creditControl.TypeOfBusiness = creditControl.TypeOfBusiness;

                    // get sender email
                    CreditControlApproveTransTable prevApprove = new CreditControlApproveTransTable();

                    if (approveTrans.ApproveLevel == 1)
                    {
                        prevApprove = await t.Connection.GetAsync<CreditControlApproveTransTable>(approveTrans.ApproveLevel, t);
                    }
                    else if (approveTrans.ApproveLevel > 1)
                    {
                        prevApprove = await t.Connection.GetAsync<CreditControlApproveTransTable>(approveTrans.ApproveLevel - 1, t);
                    }
                    else
                    {
                        prevApprove.Email = "it_ea@deestone.com";
                    }

                    if (ApproveResult == 1)
                    {
                        approveTrans.ApproveDate = DateTime.Now;
                        approveTrans.IsDone = 1;
                        approveTrans.Remark = ApproveRemark;
                    }
                    else if (ApproveResult == 2)
                    {
                        approveTrans.RejectDate = DateTime.Now;
                        approveTrans.IsDone = 0;
                        approveTrans.Remark = ApproveRemark;

                        creditControl.RequestStatus = RequestStatusModel.Reject;

                        if (user == null)
                        {
                            throw new Exception("ไม่พบ Email ของคนสร้างรายการ");
                        }

                        // send email
                        var sendNextEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                                $@"
                                    <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                    <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                    <b>สถานะ:</b> <span class='text-danger'>ยกเลิกรายการ</span> <br/>
                                    <b>Remark:</b> {ApproveRemark}
                                ",
                                new List<string> { user.Email },
                                new List<string> { },
                                prevApprove.Email
                            );

                        if (sendNextEmail.Result == false)
                        {
                            throw new Exception(sendNextEmail.Message);
                        }

                        //var emailBelowApproveLevel = approveTransAll.Where(x => x.ApproveLevel < approveTrans.ApproveLevel).ToList();

                        //if (emailBelowApproveLevel.Count > 0)
                        //{
                        //    var listEmailReject = new List<string>();

                        //    foreach (var item in emailBelowApproveLevel)
                        //    {
                        //        listEmailReject.Add(item.Email);
                        //    }

                        //    // send email
                        //    var sendNextEmail = _emailService.SendEmail(
                        //            $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                        //            $@"
                        //            <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                        //            <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                        //            <b>สถานะ:</b> <span class='text-danger'>ยกเลิกรายการ</span>
                        //        ",
                        //            listEmailReject,
                        //            new List<string> { }
                        //        );

                        //    if (sendNextEmail.Result == false)
                        //    {
                        //        throw new Exception(sendNextEmail.Message);
                        //    }
                        //}
                    }

                    // is final approve ?
                    if (creditControl.CurrentApproveStep == approveTransAll.ToList().Count && ApproveResult == 1)
                    {
                        creditControl.RequestStatus = RequestStatusModel.Complete;

                        var emailBelowApproveLevel = approveTransAll.Where(x => x.ApproveLevel < approveTrans.ApproveLevel).ToList();

                        if (emailBelowApproveLevel.Count > 0)
                        {
                            var listEmailComplete = new List<string>();

                            foreach (var item in emailBelowApproveLevel)
                            {
                                listEmailComplete.Add(item.Email);
                            }

                            // send email
                            var sendNextEmail = _emailService.SendEmail(
                                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                                    $@"
                                    <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                    <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                    <b>สถานะ:</b> <span class='text-success'>ดำเนินการเสร็จสิ้น</span>
                                ",
                                    listEmailComplete,
                                    new List<string> { },
                                    approveTrans.Email
                                );

                            if (sendNextEmail.Result == false)
                            {
                                throw new Exception(sendNextEmail.Message);
                            }
                        }
                    }
                    else if (ApproveResult == 1)
                    {
                        // update head table
                        creditControl.CurrentApproveStep += 1;
                        creditControl.RequestStatus = RequestStatusModel.WaitingForApprove;

                        // next approve trans
                        var nextApproveTrans = await unitOfWork.CreditControl.GetApproveTransByLevel(id, creditControl.CurrentApproveStep);

                        // check if have backup email
                        // ##################3
                        // var nextApproveFlow = await t.Connection.GetAsync<ApproveFlowTable>(nextApproveTrans.ApproveFlowId);
                        // if (nextApproveTrans.Urgent == 1)
                        // {
                        //   if (!string.IsNullOrEmpty(nextApproveFlow.BackupEmail))
                        //   {
                        //     nextApproveTrans.Email = nextApproveFlow.BackupEmail;
                        //   }
                        // }

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await t.Connection.InsertAsync<NonceTable>(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = DateTime.Now,
                            ExpireDate = DateTime.Now.AddDays(7),
                            IsUsed = 0
                        }, t);

                        // send email
                        var sendNextEmail = _emailService.SendEmail(
                                $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                                $@"
                                    <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                    <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveDOM?id={creditControl.Id}&tid={nextApproveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                ",
                                new List<string> { nextApproveTrans.Email },
                                new List<string> { },
                                prevApprove.Email
                            );

                        if (sendNextEmail.Result == false)
                        {
                            throw new Exception(sendNextEmail.Message);
                        }

                        nextApproveTrans.SendEmailDate = DateTime.Now;

                        await t.Connection.UpdateAsync<CreditControlApproveTransTable>(nextApproveTrans, t);
                    }

                    await t.Connection.UpdateAsync<CreditControlApproveTransTable>(approveTrans, t);
                    await t.Connection.UpdateAsync<CreditControlDOMTable>(creditControl, t);
                    await t.Connection.UpdateAsync<NonceTable>(_nonce, t);

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
                    SELECT TOP 100
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