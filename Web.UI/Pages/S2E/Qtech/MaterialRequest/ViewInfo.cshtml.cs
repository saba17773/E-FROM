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

namespace Web.UI.Pages.S2E.Qtech.MaterialRequest
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
        public int AddRMID { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public int RMReqLineID { get; set; }
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
        public async Task<IActionResult> OnGetAsync(int RMREQLineID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqLineID = RMREQLineID;
                    await GetData(RMREQLineID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }
        }
        public async Task GetData(int RMREQLineID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {



                var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMREQLineID);
                RMReqID = MaterialReqLineByID.RMREQID;
                var RMREQID = MaterialReqLineByID.RMREQID;

                if(MaterialReqLineByID.APPROVESTATUS == 2)
                {
                    PageBack = "RequestDetailCancel";
                    PageMainBack = "MainCancel";
                }
                else
                {
                    PageBack = "RequestDetail";
                    PageMainBack = "Main";
                }

                var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                var AddRMID = MaterialReqHeadByID.ADDRMID;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();


                var RequestID = AddRMHeadByID.REQUESTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = MaterialReqHeadByID.REQUESTSTATUS;
                Plant = MaterialReqHeadByID.PLANT;
                ItemGroup = MaterialReqHeadByID.ITEMGROUP;
                ItemCode = MaterialReqHeadByID.ITEMCODE;
                ItemName = MaterialReqHeadByID.ITEMNAME;
                Unit = MaterialReqHeadByID.UNIT;

                No = MaterialReqLineByID.NO;
                Department = MaterialReqLineByID.DEPARTMENT;
                SupGroup = MaterialReqLineByID.SUPGROUP;
                Qty = MaterialReqLineByID.QTY;
                RequestDate = Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");

                unitOfWork.Complete();
            }
        }
    }
}
