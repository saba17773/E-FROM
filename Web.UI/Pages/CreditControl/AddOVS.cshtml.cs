using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NPOI.XSSF.UserModel;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Constants.CreditControl;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl
{
    public class AddOVSModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public CreditControlOVSTable CreditControl_OVS { get; set; } = new CreditControlOVSTable();

        [BindProperty]
        public AddressTable Address_OVS { get; set; } = new AddressTable();

        [BindProperty]
        public string SaleName { get; set; }

        [BindProperty]
        public List<int> TypeOfCustomer { get; set; }

        [BindProperty]
        public int SaveDraft { get; set; }

        public IFormFileCollection FileUpload { get; set; }

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
        public List<SelectListItem> SalesRegionMaster { get; set; }
        public List<SelectListItem> CurrencyMaster { get; set; }
        public List<SelectListItem> DeliveryTermMaster { get; set; }
        public List<CustomerTypeByProductTable> CustomerTypeByProduct { get; set; }

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
            CreditControl_OVS.RequestDate = DateTime.Now.Date;

            CustomerTypeMaster = await GetCustomerTypeMasterAsync();
            SalesRegionMaster = await GetSalesRegionMasterAsync();
            CurrencyMaster = await GetCurrencyAsync();
            DeliveryTermMaster = await GetDeliveryTermAsync();
            CustomerTypeByProduct = await GetCustomerTypeByProductAsync();
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

        public async Task OnGetAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var t = unitOfWork.Transaction;

                var user = await t.Connection.GetAsync<UserTable>(_authService.GetClaim().UserId, t);

                var employeeInfo = await unitOfWork.Employee.GetEmployeeByEmployeeId(user.EmployeeId);

                if (employeeInfo != null)
                {
                    CreditControl_OVS.SaleEmployeeId = employeeInfo.EmployeeId;
                    SaleName = employeeInfo.Name + " " + employeeInfo.LastName;
                }

                unitOfWork.Complete();
            }

            await InitialDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
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
                    await InitialDataAsync();

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var creditControlOVSRepo = new GenericRepository<CreditControlOVSTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                    var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var numberSeqRepo = new GenericRepository<NumberSeqTable>(unitOfWork.Transaction);
                    var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<CreditControlAttachFileTable>(unitOfWork.Transaction);
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var customerTypeTransTable = new GenericRepository<CustomerTypeByProductTransTable>(unitOfWork.Transaction);
                    var checkListFileTable = new GenericRepository<CreditControlCheckListFileTable>(unitOfWork.Transaction);

                    // Update number seq
                    var numberSeq = await numberSeqRepo.GetAllAsync();

                    var numberSeqOVS = numberSeq.Where(x => x.SeqKey == nameof(NumberSeqModel.CreditControl_OVS)).FirstOrDefault();

                    numberSeqOVS.SeqValue += 1;

                    await numberSeqRepo.UpdateAsync(numberSeqOVS);

                    string reqNumber = _creditControlService.GetLatestRequestNumberAsync(numberSeqOVS.SeqValue.ToString(), "O");

                    // add ovs document
                    var newOVS = new CreditControlOVSTable
                    {
                        RequestDate = CreditControl_OVS.RequestDate,
                        CreateBy = _authService.GetClaim().UserId,
                        CreateDate = DateTime.Now,
                        RequestStatus = RequestStatusModel.Open,
                        CompanyName = CreditControl_OVS.CompanyName,
                        Branch = CreditControl_OVS.Branch,
                        TypeOfBusiness = CreditControl_OVS.TypeOfBusiness,
                        TypeOfProduct = CreditControl_OVS.TypeOfProduct,
                        IsHeadQuarter = CreditControl_OVS.IsHeadQuarter,
                        RequestType = RequestTypeModel.OVS,
                        CurrentApproveStep = 1,
                        SaleEmployeeId = CreditControl_OVS.SaleEmployeeId,
                        SaleZone = CreditControl_OVS.SaleZone,
                        Currency = CreditControl_OVS.Currency,
                        DeliveryCondition = CreditControl_OVS.DeliveryCondition,
                        DestinationPort = CreditControl_OVS.DestinationPort,
                        CreditLimited = CreditControl_OVS.CreditLimited,
                        GuaranteeOVS_Check = CreditControl_OVS.GuaranteeOVS_Check,
                        GuaranteeOVS_StandbyLCAmount = CreditControl_OVS.GuaranteeOVS_StandbyLCAmount,
                        GuaranteeOVS_IssueDate = CreditControl_OVS.GuaranteeOVS_IssueDate,
                        GuaranteeOVS_ExpireDate = CreditControl_OVS.GuaranteeOVS_ExpireDate,
                        GuaranteeOVS_SecurityDepositAmount = CreditControl_OVS.GuaranteeOVS_SecurityDepositAmount,
                        GuaranteeOVS_Other = CreditControl_OVS.GuaranteeOVS_Other,
                        CommentFromSale = CreditControl_OVS.CommentFromSale,
                        CommentFromApproval = CreditControl_OVS.CommentFromApproval,
                        RequestNumber = reqNumber
                    };

                    var addOVS = await creditControlOVSRepo.InsertAsync(newOVS);

                    // add customer type trans
                    if (TypeOfCustomer.Count > 0)
                    {
                        foreach (var item in TypeOfCustomer)
                        {
                            await customerTypeTransTable.InsertAsync(new CustomerTypeByProductTransTable
                            {
                                CCId = (int)addOVS,
                                CustomerCode = null,
                                CustomerByProductId = item,
                                CreateDate = DateTime.Now
                            });
                        }
                    }

                    // add check list file
                    if (FileCheck_1) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 1 });
                    if (FileCheck_2) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 2 });
                    if (FileCheck_3) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 3 });
                    if (FileCheck_4) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 4 });
                    if (FileCheck_5) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 5 });
                    if (FileCheck_6) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 6 });
                    if (FileCheck_7) await checkListFileTable.InsertAsync(new CreditControlCheckListFileTable { CCId = (int)addOVS, FileNo = 7 });

                    // add adress
                    var newAddress = new AddressTable {
                        CCId = (int)addOVS,
                        AddressNo = Address_OVS.AddressNo,
                        Street = Address_OVS.Street,
                        City = Address_OVS.City,
                        Country = Address_OVS.Country,
                        ZipCode = Address_OVS.ZipCode,
                        AddressType = nameof(AddressTypeModel.Address1_OVS),
                        ContactPerson1 = Address_OVS.ContactPerson1,
                        ContactPerson1_Email = Address_OVS.ContactPerson1_Email,
                        ContactPerson1_Tel = Address_OVS.ContactPerson1_Tel,
                        ContactPerson2 = Address_OVS.ContactPerson2,
                        ContactPerson2_Email = Address_OVS.ContactPerson2_Email,
                        ContactPerson2_Tel = Address_OVS.ContactPerson2_Tel
                    };

                    var addAddress = await addressRepo.InsertAsync(newAddress);

                    // get approve mapping 
                    var approveMapping = await unitOfWork.CreditControl.GetApproveGroupId(nameof(CreditControlTypeModel.OVS), _authService.GetClaim().UserId);
                    if (approveMapping == null)
                    {
                        throw new Exception("Approve mapping not match!");
                    }

                    // get all approve flow 
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

                    // insert approve transaction
                    foreach (var item in approveFlow)
                    {
                        await approveTransRepo.InsertAsync(new CreditControlApproveTransTable
                        {
                            Email = item.Email,
                            ApproveLevel = item.ApproveLevel,
                            ApproveMasterId = item.ApproveMasterId,
                            ApproveFlowId = item.Id,
                            CCId = (int)addOVS,
                        });
                    }

                    // update approve step
                    var currentRecord = await creditControlOVSRepo.GetAsync((int)addOVS);
                    currentRecord.CurrentApproveStep = 1;

                    if (SaveDraft == 1)
                    {
                        currentRecord.RequestStatus = (int)StatusModel.RequestStatus.Draft; // draft
                    }

                    await creditControlOVSRepo.UpdateAsync(currentRecord);

                    // update approve trans
                    var approveTransByCCId = await unitOfWork.CreditControl.GetApproveTransByCCId((int)addOVS);
                    var approveTransLevel1 = approveTransByCCId.Where(x => x.ApproveLevel == 1).FirstOrDefault();

                    var approveTrans = await approveTransRepo.GetAsync(approveTransLevel1.Id);

                    approveTrans.SendEmailDate = DateTime.Now;

                    await approveTransRepo.UpdateAsync(approveTrans);

                    // upload file

                    string basePath = $"wwwroot/files/credit_control/ovs/{(int)addOVS}";

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    var filePath = Path.GetTempFileName();

                    foreach (var file in FileUpload)
                    {
                        using (var stream = System.IO.File.Create($"{basePath}/{file.FileName}"))
                        {
                            await file.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new CreditControlAttachFileTable
                            {
                                CCId = (int)addOVS,
                                CCType = nameof(CreditControlTypeModel.OVS),
                                FilePath = $"{basePath}",
                                FileName = $"{file.FileName}"
                            });
                        }
                    }

                    // generate nonce
                    var nonceKey = Guid.NewGuid().ToString();

                    await nonceRepo.InsertAsync(new NonceTable
                    {
                        NonceKey = nonceKey,
                        CreateDate = DateTime.Now,
                        ExpireDate = DateTime.Now.AddDays(7),
                        IsUsed = 0
                    });

                    // get sender
                    string sender = "it_ea@deestone.com";

                    var user = await userRepo.GetAsync(_authService.GetClaim().UserId);
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
                                <b>ชื่อลูกค้า:</b> {CreditControl_OVS.CompanyName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveOVS?id={addOVS}&tid={approveTransLevel1.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
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

                    AlertSuccess = "Add OVS Success.";
                    return Redirect("/CreditControl/AddOVS");
                }

            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect("/CreditControl/AddOVS");
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