using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
    public class ViewDOMModel : PageModel
    {
        public ViewDOMViewModel ViewDOMData { get; set; }
        public CreditControlDOMTable CreditControl_DOM { get; set; }

        public AddressTable Address1 { get; set; }

        public AddressTable Address2 { get; set; }

        public string TypeOfBusiness { get; set; }
        public string TypeOfProduct { get; set; }
        public string IsHeadQuarter { get; set; }
        public string Province1 { get; set; }
        public string Province2 { get; set; }
        public string District1 { get; set; }
        public string District2 { get; set; }
        public string SubDistrict1 { get; set; }
        public string SubDistrict2 { get; set; }
        public string SaleName { get; set; }

        public List<CustomerTypeTransViewModel> CustomerTypeTrans { get; set; }

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

        public List<CreditControlAttachFileTable> AttachFile { get; set; }

        public List<CreditControl_ApproveRemarkModel> ApproveRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ViewDOMModel(
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

            AttachFile = new List<CreditControlAttachFileTable>();

            CreditControl_DOM.RequestDate = DateTime.Now.Date;
            CustomerTypeMaster = await GetCustomerTypeMasterAsync();
            CustomerTypeByProductMaster = await GetCustomerTypeByProductMasterAsync();
            ProvinceMaster = await GetProvinceMasterAsync();
            //PaymentMethodMaster = await GetPaymentMethodMasterAsync();
            PaymentTermMaster = await GetPaymentTermMasterAsync();
            TermOfDeliveryMaster = GetTermOfDeliveryMaster();

            PaymentMethodList = await GetPaymentMethodListAsync();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
                var creditControlRepo = new GenericRepository<CreditControlDOMTable>(unitOfWork.Transaction);
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                // fetch data dom
                ViewDOMData = await unitOfWork.CreditControl.GetDOMDataByCCIdAsync(id);

                var creditControl = await creditControlRepo.GetAsync(id);

                CreditControl_DOM = creditControl;

                var attachFile = await unitOfWork.CreditControl.GetFileByCCIdAsync(id, nameof(RequestTypeModel.DOM));

                AttachFile = attachFile.ToList();

                var approveTrans = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
                var approveTransByLevel = approveTrans.OrderBy(x => x.ApproveLevel).ToList();

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

                ApproveRemark = approveRemark;

                var customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(creditControl.Id);
                CustomerTypeTrans = customerTypeTrans.ToList();

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

        public async Task OnGet(int id)
        {
            await InitialDataAsync(id);
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