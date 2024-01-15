using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Planing
{
    public class IndexSODModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public List<TicketTemplateSODTable> BP { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public IndexSODModel(
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

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(TicketPermissionModel.VIEW_MASTER_BP));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Planing/IndexSOD");
            }
        }

        public async Task<IActionResult> OnPostGridAsync()
        {
            try
            {
                var _contextDB = "Ticketblock";
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(_contextDB)))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<SODGridViewModel>(@"
                        SELECT 
                            YEAR(T.AllocateMonth) AS Year,
                            MONTH(T.AllocateMonth) AS Month,
                            MU.Username AS CreateName,
                            CONVERT(varchar, T.CreateDate, 103) AS CreateDate
                        FROM SOD T
                        LEFT JOIN [EForm].[dbo].TB_User MU ON T.CreateBy = MU.Id
                        GROUP BY 
                            T.AllocateMonth,
                            MU.Username,
                            CONVERT(varchar, T.CreateDate, 103)
                        ORDER BY T.AllocateMonth DESC
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteSODAsync(int year, int month)
        {
            try
            {
                var _contextDB = "Ticketblock";
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(_contextDB)))
                {
                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM SOD 
                        WHERE YEAR(AllocateMonth) = {year}
                        AND MONTH(AllocateMonth) = {month}
                    ", null, unitOfWork.Transaction);
                    

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteSOD = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

    }
}