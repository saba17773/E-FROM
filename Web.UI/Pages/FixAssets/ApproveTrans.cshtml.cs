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

namespace Web.UI.Pages.FixAssets
{
    public class ApproveTransModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public int CCId { get; set; }
        public List<AssetsLineTable> AssetsLine { get; set; }
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
                var approveTransRepo = new GenericRepository<AssetsApproveTransTable>(unitOfWork.Transaction);
                var approveTrans = await approveTransRepo.GetAsync(tid);

                var reqTransRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                var reqTrans = await reqTransRepo.GetAsync(id);

                if (approveTrans != null)
                {
                    approveTrans.Urgent = 1; // set urgent always = 1
                    approveTrans.SendEmailDate = DateTime.Now;
                    
                    if (approveTrans.ApproveLevel == reqTrans.CurrentApproveStep)
                    {
                        // approve flow
                        //var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                        //var approveFlow = await approveFlowRepo.GetAsync(approveTrans.ApproveFlowId);

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
                        var baseUrl = $"{_configuration["Config:BaseUrl"]}/FixAssets/Approve";
                        
                        // send email
                        var AssetCategoryName = "";
                        var locationFrom = "";
                        var locationTo = "";
                        var bodyTransfer = ""; 
                        var styleLocation = "";
                        var assetsLocationRepo = new GenericRepository<AssetsLocationTable>(unitOfWork.Transaction);

                        if(reqTrans.AssetCategory==1){
                            AssetCategoryName = "ตัดทรัพย์สินถาวร";
                        }else{
                            AssetCategoryName = "โอนย้ายทรัพย์สิน";

                            var assetsLocationAll = await assetsLocationRepo.GetAllAsync();
                            var assetsLocation = assetsLocationAll.Where(x =>
                                x.Id == reqTrans.MoveFrom)
                                .ToList();
                            foreach (var item in assetsLocation)
                            {
                                locationFrom += "<b>บริษัท : </b>"+item.CompanyId+"<br>";
                                locationFrom += "<b>สถานที่ : </b>"+item.Location+"<br>";
                                locationFrom += "<b>ชั้น : </b>"+item.Floor+"<br>";
                                locationFrom += "<b>ห้อง : </b>"+item.Room+"<br>";
                            }
                            
                            var assetsLocationToAll = await assetsLocationRepo.GetAllAsync();
                            var assetsLocationTo = assetsLocationToAll.Where(x =>
                                x.Id == reqTrans.MoveTo)
                                .ToList();
                            foreach (var item in assetsLocationTo)
                            {
                                locationTo += "<b>บริษัท : </b>"+item.CompanyId+"<br>";
                                locationTo += "<b>สถานที่ : </b>"+item.Location+"<br>";
                                locationTo += "<b>ชั้น : </b>"+item.Floor+"<br>";
                                locationTo += "<b>ห้อง : </b>"+item.Room+"<br>";
                            }
                            
                            styleLocation += "<style>table, td, th {border: 1px solid black;}table {width: 100%;border-collapse: collapse;}th {background-color: #f2c94c; color: white;}</style>";
                            bodyTransfer += "<table>";
                            bodyTransfer += "<tr><th>ย้ายจากที่</th><th>ย้ายไปที่</th></tr>";
                            bodyTransfer += "<tr>";
                            bodyTransfer += "<td>"+locationFrom+"</td>";
                            bodyTransfer += "<td>"+locationTo+"</td>";
                            bodyTransfer += "</tr>";
                            bodyTransfer += "</table>"; 
                        }

                        // body mail assets
                        var assetsItemRepo = new GenericRepository<AssetsLineTable>(unitOfWork.Transaction);
                        var AssetsLineAll = await assetsItemRepo.GetAllAsync();
                        AssetsLine = AssetsLineAll.Where(x =>
                            x.AssetsId == id)
                            .ToList();

                        var style = "";
                        style += "<style>table, td, th {border: 1px solid black;}table {width: 100%;border-collapse: collapse;}th {background-color: #04AA6D; color: white;}</style>";
                        var listassets = "";
                        foreach (var item in AssetsLine)
                        {
                            listassets += "<tr>";
                            listassets += "<td>"+ item.AssetsNumber +"</td>";
                            listassets += "<td>"+ item.Qty +"</td>";
                            listassets += "<td>"+ item.Description +"</td>";
                            listassets += "</tr>";
                        }
                        var createBy = await unitOfWork.Assets.GetUser((int)id);

                        var sendUrgentEmail = _emailService.SendEmail(
                            $"ทดสอบระบบ หมายเลขคำร้อง : {reqTrans.AssetNumber} เรื่อง {AssetCategoryName}",
                            $@"
                                <b>หมายเลขคำร้อง : </b> {reqTrans.AssetNumber}<br/>
                                <b>Asset Type : </b> {AssetCategoryName}<br/>
                                <b>User : </b> {createBy.Email}<br/>
                                <b>Detail : </b><br/>
                                {styleLocation}
                                {bodyTransfer}
                                <br>
                                {style}
                                <table>
                                    <tr>
                                        <th>เลขที่ทรัพย์สินถาวร</th>
                                        <th>จำนวน</th>
                                        <th>รายละเอียด</th>
                                    </tr>
                                    {listassets}
                                </table>
                                <br/>
                                <b>ดูข้อมูล : </b> <a href='{_configuration["Config:BaseUrl"]}/FixAssets/Approve?id={id}&tid={approveTrans.Id}&level={approveTrans.ApproveLevel}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                            ",
                            new List<string> { approveTrans.Email },
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
                        CCAT.BackupEmail,
                        CCAT.Name,
                        CCAT.IsDone,
                        CCAT.SendEmailDate,
                        CCAT.ApproveDate,
                        CCAT.RejectDate,
                        CCAT.Urgent
                        FROM TB_FixAssetsApproveTrans CCAT
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
                    var approveTransRepo = new GenericRepository<AssetsApproveTransTable>(unitOfWork.Transaction);
                    
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
                                FROM TB_FixAssetsApproveTrans T
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
                    
                    if(approveTrans.Count!=approveFlow.Count){
                        foreach (var item in approveFlow)
                        {
                            var approveGetLevel = await unitOfWork.Assets.GetLevelTrans(item.ApproveLevel,item.ApproveMasterId);
                            if (approveGetLevel == null)
                            {
                                await approveTransRepo.InsertAsync(new AssetsApproveTransTable
                                {
                                    CCId = CCId,
                                    ApproveMasterId = item.ApproveMasterId,
                                    ApproveFlowId = item.Id,
                                    Email = item.Email,
                                    BackupEmail = item.BackupEmail,
                                    ApproveLevel = item.ApproveLevel,
                                    Position = item.Position,
                                    Status = item.Status,
                                    Name = item.Name,
                                    LastName = item.LastName
                                });
                            }
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
                return Redirect($@"/FixAssets/{CCId}/ApproveTrans");
            }
        }

    }

}