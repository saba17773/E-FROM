using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public List<SelectListItem> StatusList { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public IndexModel(
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
                await _authService.CanAccess(nameof(CreditControlPermissionModel.VIEW_CREDITCONTROL));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var t = unitOfWork.Transaction;

                    var SelectStatusList = t.Connection.GetAll<RequestStatusTable>(t);

                    StatusList = SelectStatusList
                        .Where(x => x.CreditControl == 1)
                        .Select(x => new SelectListItem
                        {
                            Text = x.Description,
                            Value = x.Id.ToString()
                        })
                        .ToList();
                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/CreditControl");
            }
        }

        public async Task<IActionResult> OnGetDownload(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<CreditControlAttachFileTable>(unitOfWork.Transaction);

                    var file = await attachFileRepo.GetAsync(id);

                    var filePath = $"{file.FilePath}/{file.FileName}";
                    if (!System.IO.File.Exists(filePath))
                    {
                        throw new Exception("File not found.");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/x-msdownload", file.FileName);
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Result");
            }

        }

        public async Task<IActionResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<CreditControlGridViewModel>(@"
                        SELECT
                        CC.Id,
                        CC.RequestNumber,
                        CC.RequestType,
                        E.Name + ' ' + E.LastName AS SaleName,
                        CC.CompanyName,
                        RS.Id AS RequestStatusId,
                        RS.[Description] AS RequestStatus,
                        CC.CurrentApproveStep,
                        COUNT(CCAT.Id) AS TotalApproveStep
                        FROM TB_CreditControl CC
                        LEFT JOIN TB_RequestStatus RS ON RS.Id = CC.RequestStatus
                        LEFT JOIN TB_CreditControlApproveTrans CCAT ON CCAT.CCId = CC.Id
                        LEFT JOIN TB_Employee E ON E.EmployeeId = CC.SaleEmployeeId
                        WHERE CC.CreateBy = @CreateBy OR CC.SaleEmployeeId = @EmployeeId
                        GROUP BY 
                        CC.RequestNumber,
                        CC.CompanyName,
                        RS.Id,
                        RS.[Description],
                        CC.CurrentApproveStep,
                        CC.Id,
                        E.Name,
                        E.LastName,
                        CC.RequestType
                        ORDER BY CC.Id DESC
                    ", new
                    {
                        @CreateBy = _authService.GetClaim().UserId,
                        @EmployeeId = _authService.GetClaim().EmployeeId
                    }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetAlertAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var t = unitOfWork.Transaction;

                var emailList = await unitOfWork.CreditControl.GetEmailForSendAlertApproveRequest();

                if (emailList.ToList().Count == 0)
                {
                    return new JsonResult(new { Message = "No email waiting" });
                }



                foreach (var email in emailList)
                {
                    // generate nonce
                    var nonceKey = Guid.NewGuid().ToString();

                    await t.Connection.InsertAsync<NonceTable>(new NonceTable
                    {
                        NonceKey = nonceKey,
                        CreateDate = DateTime.Now,
                        ExpireDate = DateTime.Now.AddDays(7),
                        IsUsed = 0
                    }, t);

                    // get sender
                    string sender = "it_ea@deestone.com";
                    string mailBody = "";

                    var reqDetail = await unitOfWork.CreditControl.GetRequestWaitingToApproveByEmail(email.Email);
                    if (reqDetail.ToList().Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var req in reqDetail)
                        {
                            mailBody += $@"
                                <b>เลขที่คำขอ:</b> {req.RequestNumber}<br/>
                                <b>ชื่อลูกค้า:</b> {req.CompanyName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveDOM?id={req.CCId}&tid={req.TransId}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> <br/>
                            ";
                        }
                    }


                    // send email 
                    var sendEmail = _emailService.SendEmail(
                        $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เกิน 3 วัน",
                        mailBody,
                        new List<string> { email.Email },
                        new List<string> { },
                        sender
                    );

                    if (sendEmail.Result == false)
                    {
                        throw new Exception(sendEmail.Message);
                    }
                } // end loop email 

                unitOfWork.Complete();

                return new JsonResult(new { Message = "Send Alert Successful" });
            }
        }
    }
}