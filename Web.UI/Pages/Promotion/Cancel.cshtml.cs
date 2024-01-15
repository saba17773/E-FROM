using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion
{
    public class CancelModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public PromotionDOMTable Promotion_DOM { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public CancelModel(
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
                await _authService.CanAccess(nameof(PromotionPermissionModel.VIEW_PROMOTION_CANCEL));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Promotion");
            }
        }

        public async Task<IActionResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<PromotionGridViewModel>(@"
                        SELECT 
                            P.Id,
                            P.RequestNumber,
                            P.Pattern,
                            P.RequestType,
                            P.CustomerName,
                            P.TypeOfProduct,
                            TP.ByName,
                            RS.[Description] AS RequestStatus,
                            P.CurrentApproveStep,
                            COUNT(CCAT.Id) AS TotalApproveStep
                            ,CCAT.ApproveDate
                            ,P.CancelRemark
                        FROM TB_Promotion P
                        LEFT JOIN TB_PromotionByProduct TP ON P.TypeOfProduct = TP.Id
                        LEFT JOIN TB_RequestStatus RS ON RS.Id = P.RequestStatus
                        LEFT JOIN TB_PromotionApproveTrans CCAT ON CCAT.CCId = P.Id AND P.CurrentApproveStep= CCAT.ApproveLevel 
                        AND CCAT.IsDone = 1
                        WHERE RS.[Description] = 'Cancel' 
                        GROUP BY 
                            P.Id,
                            P.RequestNumber,
                            P.Pattern,
                            P.RequestType,
                            P.CustomerName,
                            P.TypeOfProduct,
                            TP.ByName,
                            RS.Description,
                            P.CurrentApproveStep
                            ,CCAT.ApproveDate
                            ,P.CancelRemark
                        ORDER BY P.Id DESC
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