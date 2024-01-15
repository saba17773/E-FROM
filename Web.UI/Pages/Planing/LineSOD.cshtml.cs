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
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Planing
{
    public class LineSODModel : PageModel
    {

        public int Year { get; set; }
        public int Month { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public LineSODModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public void OnGet(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public async Task<IActionResult> OnPostGridAsync(int year, int month)
        {
            try
            {
                var _contextDB = "Ticketblock";
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(_contextDB)))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<SODGridViewModel>($@"
                        SELECT TOP 1000 
                            YEAR(T.AllocateMonth) AS Year
                            ,MONTH(T.AllocateMonth) AS Month
                            ,T.RequestQTY
                            ,T.ConfirmQTY
                            ,T.Custcode
                            ,T.Itemid
                            ,T.Cango
                            ,T.Out
                            ,T.CreateBy
                            ,MU.Username AS CreateName
                            ,T.CreateDate
                        FROM SOD T
                        LEFT JOIN [EForm].[dbo].[TB_User] MU ON T.CreateBy = MU.Id
                        WHERE YEAR(T.AllocateMonth) = {year}
                        AND MONTH(T.AllocateMonth) = {month}
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
    }
}