using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech.MaterialRequest
{
    public class RequestDetailCancelModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public int RMReqLineID { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int RequestStatus { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public RequestDetailCancelModel(
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
        public async Task<IActionResult> OnGetAsync(int RMREQID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqID = RMREQID;
                    await GetData(RMREQID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }
        }
        public async Task GetData(int RMREQID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                var AddRMID = MaterialReqHeadByID.ADDRMID;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID &&
                                                                            x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                            x.ISACTIVE == 1 &&
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

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostGridAsync(int RMREQID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        NO = "NO",
                        REQUESTDATE = "REQUESTDATE",
                        DEPARTMENT = "DEPARTMENT",
                        SUPGROUP = "SUPGROUP",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaterialRequestDetailGridModel>(@"
                        SELECT *
                          FROM
                          (
	                          SELECT RH.ID RMREQID,
		                             RH.ADDRMID,
		                             RL.ID RMREQLINEID,
		                             RL.NO,
		                             CONVERT(VARCHAR,RL.REQUESTDATE,103) + ' ' + CONVERT(VARCHAR,RL.REQUESTDATE,108) REQUESTDATE,
		                             RL.DEPARTMENT,
		                             RL.SUPGROUP,
		                             RL.QTY,
		                             RH.UNIT,
		                             RL.APPROVEMASTERID,
		                             RL.APPROVESTATUS,
		                             RL.APPROVEGROUPID,
		                             AG.GROUPDESCRIPTION,
		                             U.Username REQUESTBY
	                          FROM TB_S2EMaterialRequestHead RH JOIN
	                          TB_S2EMaterialRequestLine RL ON RL.RMREQID = RH.ID JOIN
	                          TB_S2EApproveGroup AG ON RL.APPROVEGROUPID = AG.ID JOIN
	                          TB_User U ON RL.CREATEBY = U.Id
                          )T
                          WHERE T.RMREQID = @RMREQID
                        AND " + filter + @" ORDER BY T.RMREQLINEID
                    ",
                    new
                    {
                        @RMREQID = RMREQID
                    }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostGridApproveTransAsync(int RMREQID, int RMREQLineID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EApproveTransGridModel>($@"
                           SELECT *
                            FROM
                            (
                                SELECT AT.ID,AT.APPROVEGROUPID,AT.APPROVEMASTERID,AT.EMAIL,AT.APPROVELEVEL
                                    ,CONVERT(VARCHAR,AT.SENDEMAILDATE,103)+' '+CONVERT(VARCHAR,AT.SENDEMAILDATE,108)SENDEMAILDATE
                                    ,CONVERT(VARCHAR,AT.APPROVEDATE,103)+' '+CONVERT(VARCHAR,AT.APPROVEDATE,108)APPROVEDATE
                                    ,CONVERT(VARCHAR,AT.REJECTDATE,103)+' '+CONVERT(VARCHAR,AT.REJECTDATE,108)REJECTDATE
                                    ,AT.ISDONE,AT.REMARK,AT.RMREQID,AT.RMREQLINEID,AG.GROUPDESCRIPTION
                                FROM TB_S2EMaterialRequestApproveTrans AT JOIN
                                TB_S2EApproveGroup AG ON AT.APPROVEGROUPID = AG.ID
                                WHERE  AT.RMREQID = {RMREQID} AND AT.RMREQLINEID = {RMREQLineID}
                            )T
                            GROUP BY T.ID,APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                                T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,T.GROUPDESCRIPTION,
                                T.APPROVEGROUPID,T.RMREQID,T.RMREQLINEID
                            ORDER BY T.ID ASC
                    ", null, unitOfWork.Transaction);
                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
