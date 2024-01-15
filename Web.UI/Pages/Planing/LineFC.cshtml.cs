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
    public class LineFCModel : PageModel
    {

        public int Year { get; set; }
        public string CustGroup { get; set; }
        public int VersionId { get; set; }
        public string Status { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public LineFCModel(
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

        public void OnGet(int year, string custgroup, int version, string status)
        {
            Year = year;
            CustGroup = custgroup;
            VersionId = version;
            Status = status;
        }

        public async Task<IActionResult> OnPostGridAsync(int year, string custgroup, int version)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<BPGridViewModel>($@"
                        SELECT TOP 1000
                            T.Year,
                            T.Month,
                            T.CustGroup,
                            T.VersionId,
                            MV.Version,
                            T.Remark,
                            T.ItemId,
                            T.Qty,
                            T.Amount,
                            MT.SetType,
                            T.Status,
                            T.CreateBy,
                            MU.Username AS CreateName,
                            T.CreateDate,
                            T.UpdateBy,
                            MUU.Username AS UpdateName,
                            T.UpdateDate
                        FROM TB_TicketTemplateForcast T
                        LEFT JOIN TB_TicketVersionMaster MV ON T.VersionId = MV.Id
                        LEFT JOIN TB_TicketSetTypeMaster MT ON T.Type = MT.Id
                        LEFT JOIN TB_User MU ON T.CreateBy = MU.Id
                        Left JOIN TB_User MUU ON T.UpdateBy = MUU.Id
                        WHERE T.Year = {year}
                        AND T.VersionId = {version}
                        AND T.CustGroup = '{custgroup}'
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