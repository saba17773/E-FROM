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

namespace Web.UI.Pages.S2E.Qtech.MaterialRequestSample
{
    public class ViewInfoModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string No { get; set; }
        [BindProperty]
        public string Department { get; set; }
        [BindProperty]
        public string SupGroup { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int AddRMSampleId { get; set; }
        [BindProperty]
        public int RMReqSamId { get; set; }
        [BindProperty]
        public int RMReqSamLineId { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public int RequestStatus { get; set; }
        [BindProperty]
        public string RequestDate { get; set; }
        public string PageBack { get; set; }
        public string PageMainBack { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public ViewInfoModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService,
         IEmailService emailService,
         IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int RMReqSamLineID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUESTSAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqSamLineId = RMReqSamLineID;
                    await GetData(RMReqSamLineID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }
        }
        public async Task GetData(int RMReqSamLineID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                var RMReqSampleLineByID = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);
               
                var RMReqSamID = RMReqSampleLineByID.RMREQSAMID;
                RMReqSamId = RMReqSamID;

                PageBack = "RequestDetail";
                PageMainBack = "Main";

                //if (RMReqSampleLineByID.APPROVESTATUS == 2)
                //{
                //    PageBack = "RequestDetailSampleCancel";
                //    PageMainBack = "MainCancel";
                //}
                //else
                //{
                //    PageBack = "RequestDetailSample";
                //    PageMainBack = "Main";
                //}

                var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMReqSamID);

                var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RequestID = AddRMSampleByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = RMReqSampleHeadByID.REQUESTSTATUS;
                Plant = AddRMSampleByID.PLANT;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Unit = NewRequestByID.UNIT;

                No = RMReqSampleLineByID.NO;
                Department = RMReqSampleLineByID.DEPARTMENT;
                SupGroup = RMReqSampleLineByID.SUPGROUP;
                Qty = RMReqSampleLineByID.QTY;
                RequestDate = Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");

                unitOfWork.Complete();
            }
        }
    }
}
