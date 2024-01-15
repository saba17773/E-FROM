using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl.ApproveTrans
{
  public class ViewModel : PageModel
  {
    public int CCId { get; set; }

    private IDatabaseContext _databaseContext;
    private IDatatableService _datatableService;
    private IAuthService _authService;
    private ICreditControlService _creditControlService;
    private IEmailService _emailService;
    private IConfiguration _configuration;

    public ViewModel(
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

    public async Task OnGet(int id, int tid, int urgent)
    {
      CCId = id;

      if (tid != 0)
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var t = unitOfWork.Transaction;

          var approveTrans = await t.Connection.GetAsync<CreditControlApproveTransTable>(tid, t);
          if (approveTrans != null)
          {
            approveTrans.Urgent = 1; // set urgent always = 1

            // if ()
            // get current step 
            var reqTrans = await t.Connection.GetAsync<CreditControlDOMTable>(approveTrans.CCId, t);
            if (reqTrans.CurrentApproveStep == approveTrans.ApproveLevel)
            {
              // approve flow
              var approveFlow = await t.Connection.GetAsync<ApproveFlowTable>(approveTrans.ApproveFlowId, t);

              // generate nonce
              var nonceKey = Guid.NewGuid().ToString();

              await t.Connection.InsertAsync<NonceTable>(new NonceTable
              {
                NonceKey = nonceKey,
                CreateDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(7),
                IsUsed = 0
              }, t);

              // base url
              var baseUrl = "";
              if (reqTrans.RequestType == "DOM")
              {
                baseUrl = $"{_configuration["Config:BaseUrl"]}/CreditControl/ApproveDOM";
              }
              else
              {
                baseUrl = $"{_configuration["Config:BaseUrl"]}/CreditControl/ApproveOVS";
              }

              // send email
              var sendUrgentEmail = _emailService.SendEmail(
                  $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {reqTrans.RequestNumber}",
                  $@"
                    <b>เลขที่คำขอ:</b> {reqTrans.RequestNumber}<br/>
                    <b>ชื่อลูกค้า:</b> {reqTrans.CompanyName}<br/>
                    <b>Link เพื่อดำเนินการ:</b> <a href='{baseUrl}?id={reqTrans.Id}&tid={approveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                    ",
                  new List<string> { (approveFlow.BackupEmail ?? approveFlow.Email) == "" ? approveFlow.Email : approveFlow.BackupEmail },
                  new List<string> { },
                  approveTrans.Email
              );

              if (sendUrgentEmail.Result == false)
              {
                throw new Exception(sendUrgentEmail.Message);
              }
            }

            await t.Connection.UpdateAsync<CreditControlApproveTransTable>(approveTrans, t);
          }

          unitOfWork.Complete();
        }
      }
    }

    public async Task<IActionResult> OnPostGridAsync(int id)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var data = await unitOfWork.Transaction.Connection.QueryAsync<ApproveTransactionGridViewModel>($@"
                        SELECT 
                        CCAT.Id,
                        CCAT.ApproveLevel,
                        CCAT.Email,
                        CCAT.Urgent,
                        AM.GroupDescription AS ApproveGroup,
                        AF.BackupEmail,
                        CCAT.Remark,
                        CCAT.IsDone,
                        CCAT.SendEmailDate,
                        CCAT.ApproveDate,
                        CCAT.RejectDate
                        FROM TB_CreditControlApproveTrans CCAT
                        LEFT JOIN TB_ApproveFlow AF ON AF.ApproveMasterId = CCAT.ApproveMasterId AND AF.ApproveLevel = CCAT.ApproveLevel
                        LEFT JOIN TB_ApproveMaster AM ON AM.Id = AF.ApproveMasterId
                        WHERE CCAT.CCId = {id}
                        ORDER BY CCAT.ApproveLevel ASC
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