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
    public class CancelModel : PageModel
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
        [BindProperty]
        public string CancelRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public CancelModel(
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
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RAWMATERIALREQUEST));

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
                return Redirect("/S2E/Qtech/MaterialRequest/Main");
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

                var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                var ADDRMID = MaterialReqHeadByID.ADDRMID;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID &&
                                                                           x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                           x.ISACTIVE == 1 &&
                                                                           x.ID != RMREQLineID &&
                                                                           x.APPROVESTATUS != 2);
                decimal QtyUse = 0;
                if (MaterialReqLineByReqID != null)
                {
                    foreach (var MaterialReqLineQTY in MaterialReqLineByReqID)
                    {
                        QtyUse += MaterialReqLineQTY.QTY;
                    }
                }

                QtyTotal = AddRMLineByID.QTY - QtyUse;

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
        public async Task<IActionResult> OnPostAsync(int RMREQLineID)
        {
            if (!ModelState.IsValid)
            {
                RMReqLineID = RMREQLineID;
                await GetData(RMREQLineID);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CancelBy = _authService.GetClaim().UserId;
                    var DatetimeNow = DateTime.Now;

                    var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMREQLineID);

                    var RMREQID = MaterialReqLineByID.RMREQID;

                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                    MaterialReqHeadByID.REQUESTSTATUS = RequestStatusModel.Open;


                    MaterialReqLineByID.CANCELREMARK = CancelRemark;
                    MaterialReqLineByID.APPROVESTATUS = RequestStatusModel.Cancel;
                    MaterialReqLineByID.COMPLETEBY = CancelBy;
                    MaterialReqLineByID.COMPLETEDATE = DatetimeNow;

                    await MaterialReqHeadRepo.UpdateAsync(MaterialReqHeadByID);
                    await MaterialReqLineRepo.UpdateAsync(MaterialReqLineByID);

                    unitOfWork.Complete();

                    AlertSuccess = "ยกเลิกสำเร็จ";
                    return Redirect($"/S2E/Qtech/MaterialRequest/{RMREQID}/RequestDetail");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Qtech/MaterialRequest/{RMREQLineID}/Cancel");
            }
        }
    }
}
