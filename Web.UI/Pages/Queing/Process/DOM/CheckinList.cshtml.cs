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
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.Queing;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Process.DOM
{
    public class CheckinListModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string PlantView { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public CheckinListModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(string plant)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    Plant = plant;

                    var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                    var PlantALL = await PlantRepo.GetAllAsync();
                    var PlantbyPlantCode = PlantALL.Where(x => x.company == plant).FirstOrDefault();

                    PlantView = PlantbyPlantCode.FullName_EN;

                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/DOM/{plant}/Index");
            }
        }
        public async Task<JsonResult> OnPostGridAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        WORKORDERNO = "WORKORDERNO",
                        TRUCKID = "TRUCKID",
                        DRIVERNAME = "DRIVERNAME",
                        ROUTE = "ROUTE",
                        STATUSDETAIL = "STATUSDETAIL"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingDOMGridModel>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT Q.ID
	                            ,Q.NO
	                            ,Q.WORKORDERNO
	                            ,Q.PLANT
	                            ,Q.TRUCKID
	                            ,Q.DRIVERID
	                            ,Q.DRIVERNAME
	                            ,Q.TRUCKCATEID
	                            ,Q.STDTIME
	                            ,Q.DRIVERTEL
	                            ,Q.PROVINCESID
	                            ,Q.STATUS
	                            ,Q.CHECKINBY
	                            ,CONVERT(NVARCHAR,Q.CHECKINDATE,103) CHECKINDATE
	                            ,S.DESCRIPTION STATUSDETAIL
	                            ,P.NameInThai PROVINCESNAME
	                            ,T.DESCRIPTION TRUCKCATEDESC
                            FROM TB_QingDOM Q JOIN
                            TB_QingMaster_Status S ON Q.STATUS = S.ID JOIN
                            TB_Provinces P ON Q.PROVINCESID = P.Id JOIN
                            TB_QingMaster_TruckCategory T ON Q.TRUCKCATEID = T.ID
                            WHERE Q.STATUS = 1 AND Q.PLANT = @plant
                        )T
                        WHERE " + filter + @"
                    ",
                    new
                    {
                        @plant = plant
                    }
                    , unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
