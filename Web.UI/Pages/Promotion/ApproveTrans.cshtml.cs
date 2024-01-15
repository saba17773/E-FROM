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

namespace Web.UI.Pages.Promotion
{
    public class ApproveTransModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public int CCId { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ApproveTransModel(
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

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                var approveTrans = await approveTransRepo.GetAsync(tid);

                var reqTransRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                var reqTrans = await reqTransRepo.GetAsync(id);
                
                if (approveTrans != null)
                {
                    approveTrans.Urgent = 1; // set urgent always = 1
                    approveTrans.SendEmailDate = DateTime.Now;
                    
                    if (approveTrans.ApproveLevel == reqTrans.CurrentApproveStep)
                    {
                        // approve flow
                        var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                        var approveFlow = await approveFlowRepo.GetAsync(approveTrans.ApproveFlowId);

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();
                        var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                        
                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = DateTime.Now,
                            ExpireDate = DateTime.Now.AddDays(30),
                            IsUsed = 0
                        });

                        // base url
                        var baseUrl = "";
                        if (reqTrans.RequestType == "DOM")
                        {
                            baseUrl = $"{_configuration["Config:BaseUrl"]}/Promotion/ApproveDOM";
                        }
                        else
                        {
                            baseUrl = $"{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS";
                        }
                        
                        // send email
                        var sendUrgentEmail = _emailService.SendEmail(
                            $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ : {reqTrans.RequestNumber}",
                            $@"
                                <b>เลขที่คำขอ:</b> {reqTrans.RequestNumber}<br/>
                                <b>ลูกค้า/Area:</b> {reqTrans.CustomerName}<br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{baseUrl}?id={reqTrans.Id}&tid={approveTrans.Id}&level={approveTrans.ApproveLevel}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                ",
                            new List<string> { approveFlow.BackupEmail },
                            new List<string> { },
                            approveTrans.Email
                        );

                        if (sendUrgentEmail.Result == false)
                        {
                            throw new Exception(sendUrgentEmail.Message);
                        }
                        await approveTransRepo.UpdateAsync(approveTrans);
                    }
                    
                }

                unitOfWork.Complete();
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
                        CCAt.Id,
                        CCAT.ApproveLevel,
                        CCAT.Email,
                        AF.BackupEmail,
                        CCAT.Remark,
                        CCAT.IsDone,
                        CCAT.SendEmailDate,
                        CCAT.ApproveDate,
                        CCAT.RejectDate,
                        CCAT.Urgent
                        FROM TB_PromotionApproveTrans CCAT
                        LEFT JOIN TB_ApproveFlow AF ON CCAT.ApproveFlowId = AF.Id
                        WHERE CCId = {id}
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

        public async Task<IActionResult> OnGetResetFlowAsync(int CCId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var promotionRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var promotion = await promotionRepo.GetAsync(CCId);
                    
                    var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                    
                    var approveTransAll = await approveTransRepo.GetAllAsync();
                    var approveTrans = approveTransAll.Where(x =>
                        x.CCId == CCId)
                        .OrderBy(x => x.ApproveLevel)
                        .ToList();
                    
                    int round=0;
                    int idTrans=0;
                    int idmasterTrans=0;
                    int levelTrans=0;
                    foreach (var item in approveTrans)
                    {
                        if(item.IsDone == 0){
                            await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                                UPDATE 
	                                T
                                SET
                                    T.Email = F.Email,
                                    T.Status = F.Status,
                                    T.Name = F.Name,
                                    T.LastName = F.LastName
                                FROM TB_PromotionApproveTrans T
                                LEFT JOIN TB_ApproveFlow F ON T.ApproveMasterId = F.ApproveMasterId AND T.ApproveLevel = F.ApproveLevel
                                WHERE T.Id={item.Id}
                            ", null, unitOfWork.Transaction);

                            if(round==0){
                                idTrans = item.Id;
                                idmasterTrans = item.ApproveMasterId;
                                levelTrans = item.ApproveLevel;
                            }
                            
                            round++;
                        }
                    }

                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var approveFlowAll = await approveFlowRepo.GetAllAsync();

                    var approveFlow = approveFlowAll.Where(x =>
                        x.ApproveMasterId == idmasterTrans &&
                        x.ApproveLevel == levelTrans &&
                        x.IsActive == 1)
                        .OrderBy(x => x.ApproveLevel)
                        .ToList();

                    if (approveFlow.Count == 0)
                    {
                        throw new Exception("Approve flow not found.");
                    }
                    Console.WriteLine(approveFlow[0].Email);
                    // generate nonce
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var nonceKey = Guid.NewGuid().ToString();

                    await nonceRepo.InsertAsync(new NonceTable
                    {
                        NonceKey = nonceKey,
                        CreateDate = DateTime.Now,
                        ExpireDate = DateTime.Now.AddDays(30),
                        IsUsed = 0
                    });
                    
                    if (promotion.PromotionRef > 0)
                    {
                        var refNumber = await promotionRepo.GetAsync(promotion.PromotionRef);

                        var sendEmail = _emailService.SendEmail(
                            $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {promotion.RequestNumber}",
                            $@"
                                <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                <b>ลูกค้า/Area : </b> {promotion.CustomerName}<br/>
                                <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={CCId}&tid={idTrans}&level={levelTrans}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                <br/><b>เอกสารอ้างอิงเลขเดิม</b><br/>
                                <b>เลขที่คำขอ : </b> {refNumber.RequestNumber}<br/>
                                <b>ชื่อเรื่อง : </b> {refNumber.Pattern}<br/>
                                <b>ลูกค้า/Area : </b> {refNumber.CustomerName}<br/>
                                <b>Link อ้างอิง:</b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/{promotion.PromotionRef}/Render'>คลิกที่นี่</a> <br/>
                            ",
                            new List<string> { approveFlow[0].Email },
                            new List<string> { },
                            _authService.GetClaim().Email
                        );
                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }
                    }
                    else
                    {
                        var sendEmail = _emailService.SendEmail(
                            $"แจ้งสถานะคำร้องขออนุมัติเพิ่ม Promotion Discount เลขที่คำขอ: {promotion.RequestNumber}",
                            $@"
                                <b>เลขที่คำขอ : </b> {promotion.RequestNumber}<br/>
                                <b>ชื่อเรื่อง : </b> {promotion.Pattern}<br/>
                                <b>ลูกค้า/Area : </b> {promotion.CustomerName}<br/>
                                <b>Link เพื่อดำเนินการ : </b> <a href='{_configuration["Config:BaseUrl"]}/Promotion/ApproveOVS?id={CCId}&tid={idTrans}&level={levelTrans}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                            ",
                            new List<string> { approveFlow[0].Email },
                            new List<string> { },
                            _authService.GetClaim().Email
                        );
                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "ResetApproveFlow Success.";
                    return new JsonResult(new { ResetFlow = 1 });

                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect($@"/Promotion/{CCId}/ApproveTrans");
            }
        }

    }

}