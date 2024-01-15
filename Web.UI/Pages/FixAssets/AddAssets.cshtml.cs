using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;
using Renci.SshNet.Messages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.FixAssets
{
    public class AddAssetsModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        
        [BindProperty]
        public AssetsTable Assets { get; set; }
        public List<SelectListItem> CompanyMaster { get; set; }
        public List<SelectListItem> LocationMaster { get; set; }
        public List<SelectListItem> FloorMaster { get; set; }
        public List<SelectListItem> RoomMaster { get; set; }
        public IFormFile UploadFileAsset { get; set; }
        [BindProperty]
        public List<string> _assetnumber { get; set; }
        [BindProperty]
        public List<string> _assetqty { get; set; }
        [BindProperty]
        public List<string> _assetdescription { get; set; }
        [BindProperty]
        public List<string> _employee { get; set; }
        [BindProperty]
        public List<string> _location { get; set; }
        public int BtnType { get; set; }
        [BindProperty]
        public int RoomLocation { get; set; }
        [BindProperty]
        public int RoomLocationTo { get; set; }
        [BindProperty]
        public string ReceiveEmployeeId { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddAssetsModel(
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

        private async Task InitialDataAsync()
        {
            Assets = new AssetsTable();
            CompanyMaster = await GetCompanyMasterAsync();
            LocationMaster = await GetLocationMasterAsync();
            FloorMaster = await GetFloorMasterAsync();
            RoomMaster = await GetRoomMasterAsync();
        }

        public async Task OnGetAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var nonceItRepo = new GenericRepository<NonceITTable>(unitOfWork.Transaction);
                
            }
            await InitialDataAsync();
        }

        public async Task<IActionResult> OnPostAsync(string draft,string save)
        {
            try
            {
                await _authService.CanAccess(nameof(AssetsPermissionModel.ADD_ASSETS));
                if (!ModelState.IsValid)
                {
                    await InitialDataAsync();

                    return Page();
                }
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                    var assetsItemRepo = new GenericRepository<AssetsLineTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<AssetsAttachFileTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);
                    var approveTransRepo = new GenericRepository<AssetsApproveTransTable>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                    var assetsLocationRepo = new GenericRepository<AssetsLocationTable>(unitOfWork.Transaction);
                    var employeeRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var employeeMapHeadRepo = new GenericRepository<AssetsMapHeadTable>(unitOfWork.Transaction);
                    
                    var numberSeqRepo = new GenericRepository<NumberSeqTable>(unitOfWork.Transaction);
                    var numberSeq = await numberSeqRepo.GetAllAsync();
                    var numberSeqMemo = numberSeq.Where(x => x.SeqKey == "Assets").FirstOrDefault();
                    numberSeqMemo.SeqValue += 1;
                    await numberSeqRepo.UpdateAsync(numberSeqMemo);

                    var telSeqRepo = new GenericRepository<TelTable>(unitOfWork.Transaction);
                    var tel = await telSeqRepo.GetAllAsync();
                    var telNumber = tel.Where(x => x.EmployeeId == _authService.GetClaim().EmployeeId).FirstOrDefault(); 

                    string prefix = "AST";
                    if(Assets.AssetCategory==1){
                        prefix = "ASD";
                    }

                    var reqNumber = _creditControlService.GetLatestRequestNumberAssetsAsync(numberSeqMemo.SeqValue.ToString(), prefix);

                    if (!string.IsNullOrEmpty(draft))
                    {
                        BtnType = 8;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        BtnType = 1;
                    }

                    var newAssets = new AssetsTable
                    {
                        AssetNumber = reqNumber,
                        AssetDate = DateTime.Now,
                        AssetType = Assets.AssetType,
                        AssetCategory = Assets.AssetCategory,
                        Company = Assets.Company,
                        AssetCondition = Assets.AssetCondition,
                        AssetCause = Assets.AssetCause,
                        CreateBy = _authService.GetClaim().UserId,
                        CreateDate = DateTime.Now,
                        RequestStatus = BtnType,
                        MoveFrom = RoomLocation,
                        MoveTo = RoomLocationTo,
                        ReceiveEmployee = ReceiveEmployeeId,
                        Phone = telNumber.TelNumber.ToString()
                    };

                    var addAssets = await assetsRepo.InsertAsync(newAssets);

                    int i = 0;
                    var listassets = "";
                    foreach (var item in _assetnumber)
                    {
                        await assetsItemRepo.InsertAsync(new AssetsLineTable
                        {
                            AssetsId = (int)addAssets,
                            AssetsNumber = item,
                            Qty = Convert.ToDouble(_assetqty[i]),
                            Description = _assetdescription[i],
                            EmployeeName = _employee[i],
                            RoomName = _location[i]
                        });
                        //Console.WriteLine(item+"_"+_assetqty[i]+"_"+_assetdescription[i]);
                        listassets += "<tr>";
                        listassets += "<td>"+ item +"</td>";
                        listassets += "<td>"+ _assetqty[i] +"</td>";
                        listassets += "<td>"+ _assetdescription[i] +"</td>";
                        listassets += "</tr>";
                        i++;
                    }

                    // Upload file.
                    string basePath = $"wwwroot/files/assets/{(int)addAssets}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    var filePath = Path.GetTempFileName();
                    if (UploadFileAsset != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileAsset.FileName.Substring(UploadFileAsset.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{reqNumber + "." + TypeFile}"))
                        {
                            await UploadFileAsset.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new AssetsAttachFileTable
                            {
                                CCId = (int)addAssets,
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{reqNumber + "." + TypeFile}"
                            });
                        }
                    }

                    //send mail
                    if (BtnType == 1)
                    {
                        // Approve Step
                        var approveMapping = await unitOfWork.Assets.GetApproveGroupId(Assets.AssetType, 1, Assets.Company);

                        if (approveMapping == null)
                        {
                            throw new Exception("Approve mapping not match!");
                        }

                        var approveFlowAll = await approveFlowRepo.GetAllAsync();
                        var approveFlow = approveFlowAll.Where(x =>
                            x.ApproveMasterId == approveMapping.ApproveMasterId &&
                            x.IsActive == 1)
                            .OrderBy(x => x.ApproveLevel)
                            .ToList();
                    
                        if (approveFlow.Count == 0)
                        {
                            throw new Exception("Approve flow not found.");
                        }

                        // emp create
                        var employeeCreateAll = await employeeRepo.GetAllAsync();
                        var employeeCreate = employeeCreateAll.Where(x =>
                        x.EmployeeId == _authService.GetClaim().EmployeeId)
                        .ToList();

                        // get manager create Lv1
                        var GetCreateApprove = await unitOfWork.Transaction.Connection.QueryAsync<EmployeeTable>($@"
                                    EXEC GetApproveLevelHR @emp_create
                                ", new { emp_create = _authService.GetClaim().EmployeeId }, unitOfWork.Transaction);

                        string Section1 = "";
                        string Section2 = "";

                        foreach (var item in GetCreateApprove)
                        {
                            Section1 = item.Approve1.ToString();
                            Section2 = item.Approve2.ToString();
                        }

                        //find receive Lv1
                        var employeeSection1 = employeeCreateAll.Where(x =>
                        x.EmployeeId == Section1)
                        .ToList();

                        //find receive Lv2
                        var employeeSection2 = employeeCreateAll.Where(x =>
                        x.EmployeeId == Section2)
                        .ToList();

                        // approve trans lv1
                        var newApproveTrans1 = new AssetsApproveTransTable
                        {
                            ApproveMasterId = approveFlow[0].ApproveMasterId,
                            ApproveLevel = (1),
                            //Email = employeeSection1[0].Email,
                            Email = "sukanya_y@deestone.com",
                            ApproveFlowId = approveFlow[0].Id,
                            CCId = (int)addAssets,
                            Position = employeeSection1[0].PositionName,
                            Status = "Approved By",
                            Name = employeeSection1[0].Name,
                            LastName = employeeSection1[0].LastName
                        };

                        // approve trans lv2
                        var newApproveTrans2 = new AssetsApproveTransTable
                        {
                            ApproveMasterId = approveFlow[0].ApproveMasterId,
                            ApproveLevel = (2),
                            //Email = employeeSection2[0].Email,
                            Email = "sukanya_y@deestone.com",
                            ApproveFlowId = approveFlow[0].Id,
                            CCId = (int)addAssets,
                            Position = employeeSection2[0].PositionName,
                            Status = "Approved By",
                            Name = employeeSection2[0].Name,
                            LastName = employeeSection2[0].LastName
                        };

                        await approveTransRepo.InsertAsync(newApproveTrans1);
                        await approveTransRepo.InsertAsync(newApproveTrans2);

                        // approve trans set
                        var approveFlowAllSet = await approveFlowRepo.GetAllAsync();
                        var approveFlowSet = approveFlowAllSet.Where(x =>
                            x.ApproveMasterId == approveMapping.ApproveMasterId &&
                            x.IsActive == 1)
                            .OrderBy(x => x.ApproveLevel)
                            .ToList();
                        foreach (var item in approveFlowSet)
                        {
                            // insert approve transaction
                            await approveTransRepo.InsertAsync(new AssetsApproveTransTable
                            {
                                Email = item.Email,
                                ApproveLevel = item.ApproveLevel,
                                ApproveMasterId = item.ApproveMasterId,
                                ApproveFlowId = item.Id,
                                CCId = (int)addAssets,
                                Position = item.Position,
                                Status = item.Status,
                                Name = item.Name,
                                LastName = item.LastName,
                                BackupEmail = item.BackupEmail
                            });
                        }

                        //โอนย้าย
                        if (Assets.AssetCategory == 2)
                        {
                            var assetsFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                            //โอนย้าย แจ้งเปลี่ยนผู้ถือครอง
                            if (Assets.AssetType == 2)
                            {
                                if (approveFlow.Count != 1)
                                {
                                    throw new Exception("Approve flow setup defective-2.");
                                }

                                // emp receive
                                var employeeAll = await employeeRepo.GetAllAsync();
                                var employee = employeeAll.Where(x =>
                                x.EmployeeId == ReceiveEmployeeId)
                                .ToList();

                                //get section receive Lv4,5
                                var GetReceiveApprove = await unitOfWork.Transaction.Connection.QueryAsync<EmployeeTable>($@"
                                    EXEC GetApproveLevelHR @emp_receive
                                ", new { emp_receive = ReceiveEmployeeId }, unitOfWork.Transaction);

                                string Section4 = "";
                                string Section5 = "";
                                foreach (var item in GetReceiveApprove)
                                {
                                    Section4 = item.Approve1.ToString();
                                    Section5 = item.Approve2.ToString();
                                }

                                //find receive Lv4
                                var employeeSection4 = employeeAll.Where(x =>
                                x.EmployeeId == Section4)
                                .ToList();

                                //find receive Lv5
                                var employeeSection5 = employeeAll.Where(x =>
                                x.EmployeeId == Section5)
                                .ToList();

                                // approve trans lv4
                                var newApproveTrans4 = new AssetsApproveTransTable
                                {
                                    ApproveMasterId = approveFlow[0].ApproveMasterId,
                                    ApproveLevel = (4),
                                    //Email = employee[0].Email,
                                    Email = "sukanya_y@deestone.com",
                                    BackupEmail = employeeSection4[0].Email,
                                    ApproveFlowId = approveFlow[0].Id,
                                    CCId = (int)addAssets,
                                    Position = employee[0].PositionName,
                                    Status = "Approved By",
                                    Name = employee[0].Name,
                                    LastName = employee[0].LastName
                                };

                                // approve trans lv5
                                var newApproveTrans5 = new AssetsApproveTransTable
                                {
                                    ApproveMasterId = approveFlow[0].ApproveMasterId,
                                    ApproveLevel = (5),
                                    //Email = employeeSection5[0].Email,
                                    Email = "sukanya_y@deestone.com",
                                    ApproveFlowId = approveFlow[0].Id,
                                    CCId = (int)addAssets,
                                    Position = employeeSection5[0].PositionName,
                                    Status = "Approved By",
                                    Name = employeeSection5[0].Name,
                                    LastName = employeeSection5[0].LastName
                                };

                                await approveTransRepo.InsertAsync(newApproveTrans4);
                                await approveTransRepo.InsertAsync(newApproveTrans5);

                            }

                            //โอนย้าย แจ้งเปลี่ยนผู้ถือครอง & เคลื่อนย้ายอุปปกรณ์
                            if (Assets.AssetType == 3)
                            {
                                if(approveFlow.Count != 3)
                                {
                                    throw new Exception("Approve flow setup defective-3.");
                                }

                                // emp receive
                                var employeeAll = await employeeRepo.GetAllAsync();
                                var employee = employeeAll.Where(x =>
                                x.EmployeeId == ReceiveEmployeeId)
                                .ToList();

                                //get section receive Lv6,7
                                var GetReceiveApprove = await unitOfWork.Transaction.Connection.QueryAsync<EmployeeTable>($@"
                                    EXEC GetApproveLevelHR @emp_receive
                                ", new { emp_receive = ReceiveEmployeeId }, unitOfWork.Transaction);

                                if (GetReceiveApprove.ToList().Count == 0)
                                {
                                    throw new Exception("Head Approve not found.");
                                }

                                string Section6 = "";
                                string Section7 = "";
                                foreach (var item in GetReceiveApprove)
                                {
                                    Section6 = item.Approve1.ToString();
                                    Section7 = item.Approve2.ToString();
                                }

                                //find receive Lv6
                                var employeeSection6 = employeeAll.Where(x =>
                                x.EmployeeId == Section6)
                                .ToList();

                                //find receive Lv7
                                var employeeSection7 = employeeAll.Where(x =>
                                x.EmployeeId == Section7)
                                .ToList();

                                // approve trans lv6
                                var newApproveTrans6 = new AssetsApproveTransTable
                                {
                                    ApproveMasterId = approveFlow[0].ApproveMasterId,
                                    ApproveLevel = (6),
                                    //Email = employee[0].Email,
                                    Email = "sukanya_y@deestone.com",
                                    BackupEmail = employeeSection6[0].Email,
                                    ApproveFlowId = approveFlow[0].Id,
                                    CCId = (int)addAssets,
                                    Position = employee[0].PositionName,
                                    Status = "Approved By",
                                    Name = employee[0].Name,
                                    LastName = employee[0].LastName
                                };

                                // approve trans lv7
                                var newApproveTrans7 = new AssetsApproveTransTable
                                {
                                    ApproveMasterId = approveFlow[0].ApproveMasterId,
                                    ApproveLevel = (7),
                                    //Email = employeeSection7[0].Email,
                                    Email = "sukanya_y@deestone.com",
                                    ApproveFlowId = approveFlow[0].Id,
                                    CCId = (int)addAssets,
                                    Position = employeeSection7[0].PositionName,
                                    Status = "Approved By",
                                    Name = employeeSection7[0].Name,
                                    LastName = employeeSection7[0].LastName
                                };

                                await approveTransRepo.InsertAsync(newApproveTrans6);
                                await approveTransRepo.InsertAsync(newApproveTrans7);

                            }

                        }

                        // update approve step
                        var currentRecord = await assetsRepo.GetAsync((int)addAssets);
                        currentRecord.CurrentApproveStep = 1;

                        await assetsRepo.UpdateAsync(currentRecord);

                    //if(BtnType == 1){
                        
                        // update approve trans
                        var approveTransByCCId = await unitOfWork.Assets.GetApproveTransByCCId((int)addAssets);
                        var approveTransLevel1 = approveTransByCCId.Where(x => x.ApproveLevel == 1).FirstOrDefault();
                        var approveLevelnext = approveTransLevel1.ApproveLevel;
                        var approveTrans = await approveTransRepo.GetAsync(approveTransLevel1.Id);
                        approveTrans.SendEmailDate = DateTime.Now;
                        await approveTransRepo.UpdateAsync(approveTrans);

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new NonceTable
                        {
                            NonceKey = nonceKey,
                            CreateDate = DateTime.Now,
                            ExpireDate = DateTime.Now.AddDays(30),
                            IsUsed = 0
                        });
                        // send mail
                        var AssetCategoryName = "";
                        var locationFrom = "";
                        var locationTo = "";
                        var bodyTransfer = ""; 
                        var styleLocation = "";

                        if(Assets.AssetCategory==1){
                            AssetCategoryName = "ตัดทรัพย์สินถาวร";
                        }else{
                            AssetCategoryName = "โอนย้ายทรัพย์สิน - แจ้งเปลี่ยนผู้ถือครอง";
                            if (Assets.AssetType==3)
                            {
                                AssetCategoryName = "โอนย้ายทรัพย์สิน - แจ้งเปลี่ยนผู้ถือครอง & เคลื่อนย้ายอุปปกรณ์";
                            }
                            
                            var assetsLocationAll = await assetsLocationRepo.GetAllAsync();
                            var assetsLocation = assetsLocationAll.Where(x =>
                                x.Id == RoomLocation)
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
                                x.Id == RoomLocationTo)
                                .ToList();
                            foreach (var item in assetsLocationTo)
                            {
                                locationTo += "<b>บริษัท : </b>"+item.CompanyId+"<br>";
                                locationTo += "<b>สถานที่ : </b>"+item.Location+"<br>";
                                locationTo += "<b>ชั้น : </b>"+item.Floor+"<br>";
                                locationTo += "<b>ห้อง : </b>"+item.Room+"<br>";
                            }

                            if (Assets.AssetType == 3)
                            {
                                styleLocation += "<style>table, td, th {border: 1px solid black;}table {width: 100%;border-collapse: collapse;}th {background-color: #f2c94c; color: white;}</style>";
                                bodyTransfer += "<table>";
                                bodyTransfer += "<tr><th>ย้ายจากที่</th><th>ย้ายไปที่</th></tr>";
                                bodyTransfer += "<tr>";
                                bodyTransfer += "<td>" + locationFrom + "</td>";
                                bodyTransfer += "<td>" + locationTo + "</td>";
                                bodyTransfer += "</tr>";
                                bodyTransfer += "</table>";
                            }
                        }
                        
                        var style = "";
                        style += "<style>table, td, th {border: 1px solid black;}table {width: 100%;border-collapse: collapse;}th {background-color: #04AA6D; color: white;}</style>";
                        var createBy = await unitOfWork.Assets.GetUser((int)addAssets);

                        var sendEmail = _emailService.SendEmail(
                            $"ระบบ หมายเลขคำร้อง : {reqNumber} เรื่อง {AssetCategoryName}",
                            $@"
                                <b>หมายเลขคำร้อง : </b> {reqNumber}<br/>
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
                                <b>ดูข้อมูล : </b> <a href='{_configuration["Config:BaseUrl"]}/FixAssets/Approve?id={addAssets}&tid={approveTransLevel1.Id}&level={approveLevelnext}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                            ",
                            //new List<string> { employeeSection1[0].Email },
                            new List<string> { "sukanya_y@deestone.com" },
                            new List<string> { },
                            _authService.GetClaim().Email
                        );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }
                    }
                    
                    unitOfWork.Complete();
                    AlertSuccess = "Add Assets Success.";
                    return Redirect("/FixAssets");
                }
                // return Redirect("/FixAssets");

            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/FixAssets/AddAssets");
            }

        }

        public async Task<List<SelectListItem>> GetCompanyMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                // var companyRepo = new GenericRepository<Company>(unitOfWork.Transaction);

                // var companyAll = await companyRepo.GetAllAsync();

                // return companyAll
                //     .Select(x => new SelectListItem
                //     {
                //         Value = x.CompanyId.ToString(),
                //         Text = x.CompanyId + " (" + x.CompanyName + ")",
                //     })
                //     .ToList();
                string company = _authService.GetClaim().CompanyGroup;
                var companyAll = await unitOfWork.Transaction.Connection.QueryAsync<Company>($@"
                        SELECT  
                        *
                        FROM TB_Company
                        WHERE
                        Id IN 
                            (
                            SELECT nstr FROM SplitStringList(
                            (SELECT GroupCompany FROM TB_FixAssetsCompanyGroup WHERE Id='{company}')
                            , ',')
                            )
                    ", null, unitOfWork.Transaction);
                    
                    unitOfWork.Complete();

                    return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.CompanyId.ToString(),
                        Text = x.CompanyId + " (" + x.CompanyName + ")",
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetLocationMasterAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var companyAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsLocationViewModel>($@"
                        SELECT  
                        *
                        FROM TB_FixAssetsLocation
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Location,
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetFloorMasterAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var companyAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsLocationViewModel>($@"
                        SELECT  
                        *
                        FROM TB_FixAssetsLocation
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Floor,
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetRoomMasterAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var companyAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsLocationViewModel>($@"
                        SELECT  
                        *
                        FROM TB_FixAssetsLocation
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Room,
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<JsonResult> OnPostAssetsGridAsync(string company)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
            {
                var field = new
                {
                    nameAlias = "NameAlias"
                };

                var filter = _datatableService.Filter(Request, field);
                
                var assetsNumber = await unitOfWork.Transaction.Connection.QueryAsync<AssetsNumberTable>($@"
                        SELECT TOP 100 NameAlias,DataareaId,AssetId,SUBSTRING(Name, 1, 50) AS Name
                        FROM ASSETTABLE
                        WHERE DataareaId = '{company}'
                        AND DSG_OBSOLETE = 0
                        AND DSG_AssetTypeID = 'IT'
                        AND " + filter + @"  
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatableService.Format(Request, assetsNumber.ToList()));
            }
        }

        public async Task<JsonResult> OnPostEmployeeGridAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var field = new
                {
                    employeeId = "employeeId"
                };

                var filter = _datatableService.Filter(Request, field);
                
                var employee = await unitOfWork.Transaction.Connection.QueryAsync<EmployeeTable>($@"
                        SELECT TOP 100 
                        [EmployeeId]
                        ,[Name]
                        ,[LastName]
                        ,[NameEng]
                        ,[Company]
                        ,[PositionCode]
                        ,[PositionName]
                        ,[DivisionCode]
                        ,[DivisionName]
                        ,[DepartmentCode]
                        ,[DepartmentName]
                        ,[Status]
                        ,[EmployeeIdOld]
                        ,[Email]
                        FROM TB_Employee
                        WHERE " + filter + @"  
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatableService.Format(Request, employee.ToList()));
            }
        }

        public async Task<IActionResult> OnGetLocationAsync(string company)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsLocationViewModel>($@"
                        SELECT  
                        [Location]
                        FROM TB_FixAssetsLocation
                        WHERE CompanyId = '{company}'
                        GROUP BY [Location]
                    ", null, unitOfWork.Transaction);
                    
                    unitOfWork.Complete();

                    return new JsonResult(ItemAll.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetLocationFloorAsync(string company,string location)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsLocationViewModel>($@"
                        SELECT  
                        [Floor]
                        FROM TB_FixAssetsLocation
                        WHERE CompanyId = '{company}'
                        AND Location = '{location}'
                        GROUP BY [Floor]
                    ", null, unitOfWork.Transaction);
                    
                    unitOfWork.Complete();

                    return new JsonResult(ItemAll.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetLocationRoomAsync(string company,string location,string floor)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsLocationViewModel>($@"
                        SELECT  
                        [Id]
                        ,[CompanyId]
                        ,[Location]
                        ,[Floor]
                        ,[Room]
                        ,[EquipmentUse]
                        FROM TB_FixAssetsLocation
                        WHERE CompanyId = '{company}'
                        AND Location = '{location}'
                        AND Floor = '{floor}'
                    ", null, unitOfWork.Transaction);
                    
                    unitOfWork.Complete();

                    return new JsonResult(ItemAll.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetRefAssetItAsync(string asset)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsItViewModel>($@"
                        EXEC GetAssetsItOnline @passet
                    ", new { passet = asset }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(ItemAll.ToList());
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