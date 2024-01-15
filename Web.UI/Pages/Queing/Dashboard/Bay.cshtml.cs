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
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Dashboard
{
    public class BayModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        public List<QingOVS_TB> QingTrans { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public BayModel(
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
        public IActionResult OnGet(string plant)
        {
            try
            {
                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetBayShowAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingBayViewModel>(@"
                        SELECT Q.TRUCKID,Q.BAYID,B.BAY,Q.WORKORDERNO,Q.LOADID
                        FROM TB_QingOVS Q
                        LEFT JOIN TB_QingMaster_Bay B ON B.ID = Q.BAYID
                        WHERE 
                        Q.BAYID <> 0
                        AND Q.ASSIGNBAYDATE IS NOT NULL
                        AND Q.CHECKOUTDATE IS NULL
                        AND B.ISSHOW = 1
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();
                    return new JsonResult(data.ToList());
                }
                // return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

    }
}
