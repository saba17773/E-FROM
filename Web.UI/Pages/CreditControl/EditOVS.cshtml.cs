using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl
{
    public class EditOVSModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public CreditControlOVSTable CreditControl_OVS { get; set; }

        [BindProperty]
        public AddressTable Address_OVS { get; set; }

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

        public List<SelectListItem> CustomerTypeMaster { get; set; }
        public List<SelectListItem> CustomerTypeByProductMaster { get; set; }
        public List<SelectListItem> SalesRegionMaster { get; set; }
        public List<SelectListItem> CurrencyMaster { get; set; }
        public List<SelectListItem> DeliveryTermMaster { get; set; }

        public bool EditCreditLimit { get; set; }

        public string SaleName { get; set; }

        public List<CustomerTypeTransViewModel> CustomerTypeTrans { get; set; }

        [BindProperty]
        public List<int> TypeOfCustomer { get; set; }

        [BindProperty]
        public int SendApprove { get; set; }

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
            CreditControl_OVS = new CreditControlOVSTable();
            Address_OVS = new AddressTable();

            CreditControl_OVS.RequestDate = DateTime.Now.Date;
            CustomerTypeByProductMaster = await GetCustomerTypeByProductMasterAsync();
            CustomerTypeMaster = await GetCustomerTypeMasterAsync();
            SalesRegionMaster = await GetSalesRegionMasterAsync();
            CurrencyMaster = await GetCurrencyAsync();
            DeliveryTermMaster = await GetDeliveryTermAsync();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var creditControlRepo = new GenericRepository<CreditControlOVSTable>(unitOfWork.Transaction);
                var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
                var customerTypeTrans = new GenericRepository<CustomerTypeByProductTransTable>(unitOfWork.Transaction);
                var customerByProduct = new GenericRepository<CustomerTypeByProductTable>(unitOfWork.Transaction);

                CreditControl_OVS = await creditControlRepo.GetAsync(id);
                Address_OVS = await unitOfWork.CreditControl.GetAddressByCCIdAsync(id, nameof(AddressTypeModel.Address1_OVS));

                if (CreditControl_OVS.RequestStatus == RequestStatusModel.Complete)
                {
                    throw new Exception("รายการถูกดำเนินการเสร็จสิ้นไปแล้ว ไม่สามารถแก้ไขได้");
                }
                else if (CreditControl_OVS.RequestStatus == RequestStatusModel.Cancel)
                {
                    throw new Exception("รายการถูกยกเลิกไปแล้ว ไม่สามารถแก้ไขได้");
                }
                else if (CreditControl_OVS.RequestStatus == RequestStatusModel.WaitingForApprove)
                {
                    throw new Exception("รายการอยู่ระหว่างการอนุมัติ ไม่สามารถแก้ไขได้");
                }

                var employee = await unitOfWork.Employee.GetEmployeeByEmployeeId(CreditControl_OVS.SaleEmployeeId);

                if (employee != null)
                {
                    SaleName = employee.Name + " " + employee.LastName;
                }

                var approveFlow = await unitOfWork.CreditControl.GetApproveFlowByCCIdAsync(CreditControl_OVS.Id);
                var creditLimitEnable = approveFlow.Where(x => x.EditCreditLimit == 1);

                if (creditLimitEnable.FirstOrDefault() != null && creditLimitEnable.Any(x => x.Email == _authService.GetClaim().Email))
                {
                    EditCreditLimit = true;
                }
                else
                {
                    EditCreditLimit = false;
                }

                var _customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(CreditControl_OVS.Id);
                CustomerTypeTrans = _customerTypeTrans.ToList();

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
                return Redirect("/CreditControl");
            }
            
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(CreditControlPermissionModel.ADD_OVS_CREDITCONTROL));

                if (TypeOfCustomer.Count == 0)
                {
                    ModelState.AddModelError("", "Please enter Type Of Product");
                }

                if (!ModelState.IsValid)
                {
                    await InitialDataAsync(id);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var creditControlOVSRepo = new GenericRepository<CreditControlOVSTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var numberSeqRepo = new GenericRepository<NumberSeqTable>(unitOfWork.Transaction);
                    var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<CreditControlAttachFileTable>(unitOfWork.Transaction);
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var customerTypeTransTable = new GenericRepository<CustomerTypeByProductTransTable>(unitOfWork.Transaction);
                    var checkListFileTable = new GenericRepository<CreditControlCheckListFileTable>(unitOfWork.Transaction);

                    // update ovs document
                    var creditControl = await creditControlOVSRepo.GetAsync(id);
                    
                    creditControl.CompanyName = CreditControl_OVS.CompanyName;
                    creditControl.Branch = CreditControl_OVS.Branch;
                    creditControl.TypeOfBusiness = CreditControl_OVS.TypeOfBusiness;
                    creditControl.TypeOfProduct = CreditControl_OVS.TypeOfProduct;
                    creditControl.IsHeadQuarter = CreditControl_OVS.IsHeadQuarter;
                    creditControl.SaleEmployeeId = CreditControl_OVS.SaleEmployeeId;
                    creditControl.SaleZone = CreditControl_OVS.SaleZone;
                    creditControl.Currency = CreditControl_OVS.Currency;
                    creditControl.DeliveryCondition = CreditControl_OVS.DeliveryCondition;
                    creditControl.DestinationPort = CreditControl_OVS.DestinationPort;
                    creditControl.CreditLimited = CreditControl_OVS.CreditLimited;
                    creditControl.GuaranteeOVS_Check = CreditControl_OVS.GuaranteeOVS_Check;
                    creditControl.GuaranteeOVS_StandbyLCAmount = CreditControl_OVS.GuaranteeOVS_StandbyLCAmount;
                    creditControl.GuaranteeOVS_IssueDate = CreditControl_OVS.GuaranteeOVS_IssueDate;
                    creditControl.GuaranteeOVS_ExpireDate = CreditControl_OVS.GuaranteeOVS_ExpireDate;
                    creditControl.GuaranteeOVS_SecurityDepositAmount = CreditControl_OVS.GuaranteeOVS_SecurityDepositAmount;
                    creditControl.GuaranteeOVS_Other = CreditControl_OVS.GuaranteeOVS_Other;
                    creditControl.CommentFromSale = CreditControl_OVS.CommentFromSale;
                    creditControl.CommentFromApproval = CreditControl_OVS.CommentFromApproval;
                    creditControl.UpdateBy = _authService.GetClaim().UserId;
                    creditControl.UpdateDate = DateTime.Now;

                    await creditControlOVSRepo.UpdateAsync(creditControl);

                    var customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(creditControl.Id);

                    // delete old selected
                    await unitOfWork.Transaction.Connection.ExecuteAsync(@"
                        DELETE FROM TB_CustomerTypeTrans WHERE CCId = @CCId
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

                    // update address
                    var address = await unitOfWork.CreditControl.GetAddressByCCIdAsync(id, nameof(AddressTypeModel.Address1_OVS));

                    address.AddressNo = Address_OVS.AddressNo;
                    address.Street = Address_OVS.Street;
                    address.City = Address_OVS.City;
                    address.Country = Address_OVS.Country;
                    address.ZipCode = Address_OVS.ZipCode;
                    address.AddressType = nameof(AddressTypeModel.Address1_OVS);
                    address.ContactPerson1 = Address_OVS.ContactPerson1;
                    address.ContactPerson1_Email = Address_OVS.ContactPerson1_Email;
                    address.ContactPerson1_Tel = Address_OVS.ContactPerson1_Tel;
                    address.ContactPerson2 = Address_OVS.ContactPerson2;
                    address.ContactPerson2_Email = Address_OVS.ContactPerson2_Email;
                    address.ContactPerson2_Tel = Address_OVS.ContactPerson2_Tel;

                    await addressRepo.UpdateAsync(address);

                    // upload file
                    string basePath = $"wwwroot/files/credit_control/ovs/{id}";

                    if (!System.IO.Directory.Exists(basePath))
                    {
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
                                CCType = nameof(CreditControlTypeModel.OVS),
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

                        var currentApproveTrans = currentApproveTransByCCId.OrderBy(x => x.ApproveLevel).Where(x => x.IsDone == 0).FirstOrDefault();

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
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveOVS?id={id}&tid={currentApproveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
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

                        var currentApproveTransByCCId = await unitOfWork.CreditControl.GetApproveTransByCCId(id);

                        var currentApproveTrans = currentApproveTransByCCId.OrderBy(x => x.ApproveLevel).Where(x => x.IsDone == 0).FirstOrDefault();

                        // get editor email
                        var editorEmail = await userRepo.GetAsync((int)creditControl.CreateBy);

                        // send email level 1 to approve
                        var sendEmail = _emailService.SendEmail(
                            $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                            $@"
                                <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveOVS?id={id}&tid={currentApproveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
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

                    // commit
                    unitOfWork.Complete();

                    AlertSuccess = "Edit Success.";

                    if (creditControl.RequestStatus == RequestStatusModel.Reject)
                    {
                        return Redirect($@"/CreditControl");
                    }
                    else
                    {
                        return Redirect($@"/CreditControl/{id}/EditOVS");
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                await InitialDataAsync(id);

                return Page();
            }
        }

        private async Task<List<SelectListItem>> GetCustomerTypeMasterAsync()
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

        private async Task<List<SelectListItem>> GetCurrencyAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var currencyRepo = new GenericRepository<CurrencyTable>(unitOfWork.Transaction);

                var currencyAll = await currencyRepo.GetAllAsync();

                return currencyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CURRENCYCODE + " " + x.TXT
                    })
                    .ToList();
            }
        }

        private async Task<List<SelectListItem>> GetDeliveryTermAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var deliveryTermRepo = new GenericRepository<DeliveryTermTable>(unitOfWork.Transaction);

                var deliveryTermAll = await deliveryTermRepo.GetAllAsync();

                return deliveryTermAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Code + " (" + x.Txt + ")"
                    })
                    .ToList();
            }
        }

        private async Task<List<SelectListItem>> GetSalesRegionMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var salesRegionRepo = new GenericRepository<SalesRegionTable>(unitOfWork.Transaction);

                var salesRegionAll = await salesRegionRepo.GetAllAsync();

                return salesRegionAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.DSG_SALESREGIONID + " " + x.DSG_SALESREGIONNAME
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
    }
}