using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
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

namespace Web.UI.Pages.FixAssets
{
    public class ApproveModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public AssetsTable Assets { get; set; }
        public string AssetsDateStr { get; set; }
        public string AssetsCompanyStr { get; set; }
        public string AssetsNameThaiStr { get; set; }
        public string AssetsNameEngStr { get; set; }
        public string AssetsDivisionStr { get; set; }
        public string AssetsDepartmentStr { get; set; }
        public string AssetsPositionStr { get; set; }
        public string AssetsCompanyNameStr { get; set; }
        public string txtAssetCondition { get; set; }
        public string txtAssetCause { get; set; }
        public List<AssetsLocationTable> AssetsLocationFrom { get; set; }
        public List<AssetsLocationTable> AssetsLocationTo { get; set; }
        public List<AssetsLineTable> AssetsLine { get; set; }
        public List<AssetsAttachFileTable> AttachFile { get; set; }
        public List<EmployeeTable> Employee { get; set; }
        public string txtEmployeeReceive { get; set; }
        public string txtEmployeeReceiveEng { get; set; }
        public string txtEmployeeDivision { get; set; }
        public string txtEmployeeDepartment { get; set; }
        public string txtEmployeePosition { get; set; }
        public string txtEmployeeCompanyName { get; set; }
        public string txtEmployeeTel { get; set; }
        public string txtAssetsLocationTo { get; set; }
        public List<FixAssetsItViewModel> ListAssetsReceive { get; set; }

        [BindProperty]
        [Required]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }
        [BindProperty]
        public int Disposition { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ApproveModel(
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

        private async Task InitialDataAsync(int id, int tid, int level)
        {
            Assets = new AssetsTable();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                var assets = await assetsRepo.GetAsync(id);

                Assets.AssetNumber = assets.AssetNumber;
                AssetsDateStr = Convert.ToDateTime(assets.AssetDate).ToString("dd/MM/yyyy");
                var CompanyName = await unitOfWork.Assets.GetCompany(assets.Company);
                AssetsCompanyStr = CompanyName.CompanyName;
                var User = await unitOfWork.Assets.GetUser(assets.Id);
                AssetsNameThaiStr = User.NameThai;
                AssetsNameEngStr = User.NameEng;
                AssetsDivisionStr = User.DivisionName;
                AssetsDepartmentStr = User.DepartmentName;
                AssetsPositionStr = User.PositionName;
                AssetsCompanyNameStr = User.CompanyName;
                Assets.AssetCategory = assets.AssetCategory;
                Assets.AssetType = assets.AssetType;
                Assets.Phone = assets.Phone;

                if(assets.AssetCondition!=null){
                    txtAssetCondition = assets.AssetCondition.Replace("\n","<br>");
                }
                if(assets.AssetCause!=null){
                    txtAssetCause = assets.AssetCause.Replace("\n","<br>");
                }

                var assetsLocationRepo = new GenericRepository<AssetsLocationTable>(unitOfWork.Transaction);

                var assetsLocationAll = await assetsLocationRepo.GetAllAsync();
                AssetsLocationFrom = assetsLocationAll.Where(x =>
                    x.Id == assets.MoveFrom)
                    .ToList();
                var assetsLocationToAll = await assetsLocationRepo.GetAllAsync();
                AssetsLocationTo = assetsLocationToAll.Where(x =>
                    x.Id == assets.MoveTo)
                    .ToList();
                if(assets.AssetCategory == 2 && assets.AssetType == 3){
                    txtAssetsLocationTo = AssetsLocationTo[0].Room;
                }

                var assetsItemRepo = new GenericRepository<AssetsLineTable>(unitOfWork.Transaction);
                var AssetsLineAll = await assetsItemRepo.GetAllAsync();
                AssetsLine = AssetsLineAll.Where(x =>
                    x.AssetsId == assets.Id)
                    .ToList();

                //var ListAssetsReceive = new List<string>();
                //foreach (var item in AssetsLineAll)
                //{
                //    var _ListAssetsReceive = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsItViewModel>($@"
                //        EXEC GetAssetsItOnline @passet
                //    ", new { passet = item.AssetsNumber }, unitOfWork.Transaction);
                //    //ListAssetsReceive = _ListAssetsReceive.ToList();
                //    //ListAssetsReceive.Add(_ListAssetsReceive[0].EployeeName);
                //    foreach (var l in _ListAssetsReceive)
                //    {
                //        ListAssetsReceive.Add(l.EmployeeName);
                //    }
                //}

                var assetsFileRepo = new GenericRepository<AssetsAttachFileTable>(unitOfWork.Transaction);
                var AttachFileAll = await assetsFileRepo.GetAllAsync();
                AttachFile = AttachFileAll.Where(x =>
                    x.CCId == assets.Id)
                    .ToList();

                if(assets.AssetCategory == 2){
                    var assetsEmployeeRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmployeeAll = await assetsEmployeeRepo.GetAllAsync();
                    Employee = EmployeeAll.Where(x =>
                        x.EmployeeId == assets.ReceiveEmployee
                    ).ToList();
                    txtEmployeeReceive = Employee[0].Name+" "+Employee[0].LastName;
                    txtEmployeeReceiveEng = Employee[0].NameEng;
                    txtEmployeeDivision = Employee[0].DivisionName;
                    txtEmployeeDepartment = Employee[0].DepartmentName;
                    txtEmployeePosition = Employee[0].PositionName;
                    var companyRepo = new GenericRepository<Company>(unitOfWork.Transaction);
                    var company = await companyRepo.GetAllAsync();
                    var companyAll = company.Where(x => x.CompanyId == Employee[0].Company).FirstOrDefault();
                    txtEmployeeCompanyName = companyAll.CompanyName.ToString();

                    var telSeqRepo = new GenericRepository<TelTable>(unitOfWork.Transaction);
                    var tel = await telSeqRepo.GetAllAsync();
                    var telNumber = tel.Where(x => x.EmployeeId == assets.ReceiveEmployee).FirstOrDefault();
                    
                    if(telNumber != null)
                    {
                        txtEmployeeTel = telNumber.TelNumber.ToString();
                    }
                }
                
                unitOfWork.Complete();
            }
        }

        public async Task OnGet(int id, int tid, int level, string nonce)
        {
            await InitialDataAsync(id, tid, level);
        }

        public async Task<IActionResult> OnPostAsync(int id, int tid, int level, string nonce)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะ อนุมัติ หรือ ไม่อนุมัติ";
                }

                if (!ModelState.IsValid)
                {
                    return Redirect($"/Result");
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveTransRepo = new GenericRepository<AssetsApproveTransTable>(unitOfWork.Transaction);
                    //var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<AssetsAttachFileTable>(unitOfWork.Transaction);

                    // check nonce
                    var _nonce = await unitOfWork.Nonce.GetNonceByKey(nonce);

                    if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
                    {
                        throw new Exception("Nonce expired.");
                    }

                    _nonce.IsUsed = 1;

                    var approveTrans = await approveTransRepo.GetAsync(tid);
                    var assets = await assetsRepo.GetAsync(id);
                    var approveTransAll = await unitOfWork.Assets.GetApproveTransByCCId(id);
                    
                    if (assets.RequestStatus == 2)
                    {
                        // AlertError = "รายการนี้ Cancel แล้ว";
                        throw new Exception("รายการนี้ Cancel แล้ว");
                    }

                    // update approve trans
                    approveTrans.IsDone = 1;
                    approveTrans.Remark = ApproveRemark;
                    //Console.WriteLine(Disposition);
                    if(assets.AssetCategory==1){
                        approveTrans.Disposition = Disposition;
                    }

                    if (ApproveResult == 1)
                    {
                        approveTrans.ApproveDate = DateTime.Now;
                        assets.RequestStatus = RequestStatusModel.WaitingForApprove;
                    }
                    else if (ApproveResult == 2)
                    {
                        approveTrans.RejectDate = DateTime.Now;
                        assets.RequestStatus = RequestStatusModel.Reject;
                        approveTrans.IsDone = 0;
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
                    var createBy = await unitOfWork.Assets.GetUser(id);
                    var AssetCategoryName = "";
                    var locationFrom = "";
                    var locationTo = "";
                    var bodyTransfer = ""; 
                    if(assets.AssetCategory==1){
                        AssetCategoryName = "ตัดทรัพย์สินถาวร";
                    }else{
                        AssetCategoryName = "โอนย้ายทรัพย์สิน - แจ้งเปลี่ยนผู้ถือครอง";
                        if (assets.AssetType == 3)
                        {
                            AssetCategoryName = "โอนย้ายทรัพย์สิน - แจ้งเปลี่ยนผู้ถือครอง & เคลื่อนย้ายอุปปกรณ์";
                        }

                        var assetsLocationRepo = new GenericRepository<AssetsLocationTable>(unitOfWork.Transaction);
                        var assetsLocationAll = await assetsLocationRepo.GetAllAsync();
                        var assetsLocation = assetsLocationAll.Where(x =>
                            x.Id == assets.MoveFrom)
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
                            x.Id == assets.MoveTo)
                            .ToList();
                        foreach (var item in assetsLocationTo)
                        {
                            locationTo += "<b>บริษัท : </b>"+item.CompanyId+"<br>";
                            locationTo += "<b>สถานที่ : </b>"+item.Location+"<br>";
                            locationTo += "<b>ชั้น : </b>"+item.Floor+"<br>";
                            locationTo += "<b>ห้อง : </b>"+item.Room+"<br>";
                        }

                        if (assets.AssetType == 3)
                        {
                            bodyTransfer += "<table>";
                            bodyTransfer += "<tr><th>ย้ายจากที่</th><th>ย้ายไปที่</th></tr>";
                            bodyTransfer += "<tr>";
                            bodyTransfer += "<td>" + locationFrom + "</td>";
                            bodyTransfer += "<td>" + locationTo + "</td>";
                            bodyTransfer += "</tr>";
                            bodyTransfer += "</table>";
                        }

                    }

                    // is final approve ?
                    if (assets.CurrentApproveStep == approveTransAll.ToList().Count)
                    {
                        if (ApproveResult == 1)
                        {
                            assets.RequestStatus = RequestStatusModel.Complete;

                            var nextApproveTrans = await unitOfWork.Assets.GetApproveTransByLevel(id, assets.CurrentApproveStep);

                            var emailReviewed = new List<string>();

                            if(assets.AssetCategory == 1){
                                var approveTransReviewed = await unitOfWork.Assets.GetApproveTransByReviewed(id);
                                var Reviewed = approveTransReviewed.ToList();
                                emailReviewed.Add(Reviewed[0].Email);
                            }

                            var approveTransLV3 = await unitOfWork.Assets.GetApproveTransByLV3(id);
                            var ReviewedLV3 = approveTransLV3.ToList();
                            emailReviewed.Add(ReviewedLV3[0].Email);
                            // emailProd.Add("harit_j@deestone.com");
                            // Console.WriteLine(emailReviewed);
                            var sendCompleteEmail = _emailService.SendEmail(
                                    $"อนุมัติสำเร็จ : หมายเลขคำร้อง {assets.AssetNumber} เรื่อง {AssetCategoryName}",
                                    $@"
                                    <b>หมายเลขคำร้อง : </b> {assets.AssetNumber}<br/>
                                    <b>Asset Type : </b> {AssetCategoryName}<br/>
                                    <b>User : </b> {createBy.Email}<br/>
                                    <b>Detail : </b><br/>
                                    {style}
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
                                ",
                                    new List<string> { createBy.Email },
                                    // new List<string> { },
                                    emailReviewed,
                                    nextApproveTrans.Email
                                );

                            if (sendCompleteEmail.Result == false)
                            {
                                throw new Exception(sendCompleteEmail.Message);
                            }
                        }
                        else
                        {
                            var assetsTrans = await unitOfWork.Assets.GetUser(id);
                            var approveTransEmail = await approveTransRepo.GetAllAsync();
                            var email = approveTransEmail.Where(x =>
                                x.CCId == id &&
                                x.ApproveLevel == level)
                            .FirstOrDefault();

                            var sendRejectEmail = _emailService.SendEmail(
                                    $"REJECT : หมายเลขคำร้อง {assets.AssetNumber} เรื่อง {AssetCategoryName}",
                                    $@"
                                    <b>หมายเลขคำร้อง : </b> {assets.AssetNumber}<br/>
                                    <b>Asset Type : </b> {AssetCategoryName}<br/>
                                    <b>User : </b> {createBy.Email}<br/>
                                    <b>Detail : </b><br/>
                                    {style}
                                    <table>
                                        <tr>
                                            <th>เลขที่ทรัพย์สินถาวร</th>
                                            <th>จำนวน</th>
                                            <th>รายละเอียด</th>
                                        </tr>
                                        {listassets}
                                    </table>
                                    ",
                                    new List<string> { assetsTrans.Email },
                                    new List<string> { },
                                    email.Email
                                );
                            if (sendRejectEmail.Result == false)
                            {
                                throw new Exception(sendRejectEmail.Message);
                            }
                        }

                    }
                    else
                    {
                        // update head table
                        var nextApproveTrans = await unitOfWork.Assets.GetApproveTransByLevel(id, assets.CurrentApproveStep);
                        if (ApproveResult == 1)
                        {
                            assets.CurrentApproveStep += 1;
                            nextApproveTrans = await unitOfWork.Assets.GetApproveTransByLevel(id, assets.CurrentApproveStep);
                        }

                        // next approve trans

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = DateTime.Now,
                            ExpireDate = DateTime.Now.AddDays(30),
                            IsUsed = 0
                        });

                        var approveTransEmail = await approveTransRepo.GetAllAsync();
                        var email = approveTransEmail.Where(x =>
                            x.CCId == id &&
                            x.ApproveLevel == level)
                            .FirstOrDefault();

                        if (ApproveResult == 2)
                        {
                            var assetsTrans = await unitOfWork.Assets.GetUser(id);
                            var sendRejectEmail = _emailService.SendEmail(
                                $"ระบบ REJECT : หมายเลขคำร้อง {assets.AssetNumber} เรื่อง {AssetCategoryName}",
                                    $@"
                                    <b>หมายเลขคำร้อง : </b> {assets.AssetNumber}<br/>
                                    <b>Asset Type : </b> {AssetCategoryName}<br/>
                                    <b>User : </b> {createBy.Email}<br/>
                                    <b>Detail : </b><br/>
                                    {style}
                                    <table>
                                        <tr>
                                            <th>เลขที่ทรัพย์สินถาวร</th>
                                            <th>จำนวน</th>
                                            <th>รายละเอียด</th>
                                        </tr>
                                        {listassets}
                                    </table>
                                    <b>หมายเหตุ : </b> {ApproveRemark}<br/>
                                ",
                                new List<string> { assetsTrans.Email },
                                new List<string> { },
                                email.Email
                            );
                            if (sendRejectEmail.Result == false)
                            {
                                throw new Exception(sendRejectEmail.Message);
                            }
                        }

                        if (ApproveResult == 1)
                        {
                            // assets.CurrentApproveStep += 1;
                            var emailProd = new List<string>();
                            if (nextApproveTrans.Email != null)
                            {
                                emailProd.Add(nextApproveTrans.Email);
                            }
                            
                            var emailProdCC = new List<string>();
                            if (nextApproveTrans.BackupEmail != null)
                            {
                                emailProd.Add(nextApproveTrans.BackupEmail);
                            }

                            //if(assets.AssetCategory == 2 && nextApproveTrans.ApproveLevel == 4){
                            //    var approveFlowAll = await approveFlowRepo.GetAllAsync();
                            //    var approveFlow = approveFlowAll.Where(x =>
                            //    x.ApproveMasterId == nextApproveTrans.ApproveMasterId &&
                            //    x.IsActive == 1 &&
                            //    x.ApproveLevel == 4)
                            //    .ToList();
                            //    emailProd.Add(approveFlow[0].BackupEmail);
                            //    // Console.WriteLine(approveFlow[0].BackupEmail);
                            //}

                            var sendNextEmail = _emailService.SendEmail(
                                $"ระบบ หมายเลขคำร้อง : {assets.AssetNumber} เรื่อง {AssetCategoryName}",
                                $@"
                                    <b>หมายเลขคำร้อง : </b> {assets.AssetNumber}<br/>
                                    <b>Asset Type : </b> {AssetCategoryName}<br/>
                                    <b>User : </b> {createBy.Email}<br/>
                                    <b>Detail : </b><br/>
                                    {style}
                                    {bodyTransfer}
                                    <br>
                                    <table>
                                        <tr>
                                            <th>เลขที่ทรัพย์สินถาวร</th>
                                            <th>จำนวน</th>
                                            <th>รายละเอียด</th>
                                        </tr>
                                        {listassets}
                                    </table>
                                    <br/>
                                    <b>ดูข้อมูล : </b> <a href='{_configuration["Config:BaseUrl"]}/FixAssets/Approve?id={assets.Id}&tid={nextApproveTrans.Id}&level={level + 1}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                                ",
                                emailProd,
                                emailProdCC,
                                email.Email
                            );

                            if (sendNextEmail.Result == false)
                            {
                                throw new Exception(sendNextEmail.Message);
                            }
                        }

                        nextApproveTrans.SendEmailDate = DateTime.Now;

                        await approveTransRepo.UpdateAsync(nextApproveTrans);
                    }

                    await approveTransRepo.UpdateAsync(approveTrans);
                    await assetsRepo.UpdateAsync(assets);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();

                    AlertSuccess = "ดำเนินการเสร็จสิ้น";
                    return Redirect($"/Result");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Result");
            }
        }

    }
}