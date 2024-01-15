using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

namespace Web.UI.Pages.FixAssets
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public string noncekey { get; set; }
        public AssetsTable Assets { get; set; }
        public List<SelectListItem> CompanyMaster { get; set; }
        public List<FixAssetsGridViewModel> dataAssets { get; set; } = new List<FixAssetsGridViewModel>();
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
                await _authService.CanAccess(nameof(AssetsPermissionModel.VIEW_ASSETS));
                CompanyMaster = await GetCompanyMasterAsync();

                //using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                //{
                //var nonceItRepo = new GenericRepository<NonceITTable>(unitOfWork.Transaction);
                //var nonceItAll = await nonceItRepo.GetAllAsync();
                //var nonceIt = nonceItAll.Where(x =>
                //    x.NonceKey == nonce &&
                //    x.IsUsed == 0 &&
                //    x.ExpireDate >= DateTime.Now)
                //    .ToList();

                //if (nonceIt.Count == 0)
                //{
                //    throw new Exception("Authentication not found.");
                //}

                //noncekey = nonceIt[0].NonceKey;
                //}

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/FixAssets");
            }
        }

        public async Task<IActionResult> OnPostGridAsync(
            string S_AssetsNumber,
            string S_AssetsDate,
            string S_AssetCategory,
            string S_AssetsCompany,
            string S_AssetsCreateBy, 
            string S_AssetsStatus,
            string S_AssetsKeyNumber
        )
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var employeeRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var employeeAll = await employeeRepo.GetAllAsync();
                    var employee = employeeAll.Where(x => x.EmployeeId == _authService.GetClaim().EmployeeId).FirstOrDefault();

                    string company = _authService.GetClaim().CompanyGroup;
                    int createBy = _authService.GetClaim().UserId;
                    
                    var lineId = new List<int>();
                    var listlineId = await unitOfWork.Assets.GetAssetsLine(S_AssetsKeyNumber);
                    foreach (var item in listlineId)
                    {
                        lineId.Add(item.AssetsId);
                    }
                    string IdLine = string.Join(",", lineId);
                    //Debug.WriteLine(IdLine);
                    var dataAssets = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsGridViewModel>($@"
                        SELECT F.[Id]
                                ,F.[AssetNumber]
                                ,CONVERT(varchar, F.[AssetDate], 103) [AssetDate]
                                ,CASE 
                                        WHEN F.AssetType=1 THEN 'ตัดทรัพย์สินถาวร'
                                        WHEN F.AssetType=2 THEN 'แจ้งเปลี่ยนผู้ถือครอง'
                                        WHEN F.AssetType=3 THEN 'แจ้งเปลี่ยนผู้ถือครอง & เคลื่อนย้ายอุปกรณ์'
                                    ELSE '-' END [AssetType]
                                ,CASE 
                                        WHEN F.AssetCategory=1 THEN 'ตัดทรัพย์สินถาวร'
                                        WHEN F.AssetCategory=2 THEN 'โอนย้ายทรัพย์สิน'
                                    ELSE '-' END [AssetCategory]
                                ,F.[CurrentApproveStep]
                                ,RS.[Description] AS RequestStatus
                                ,F.[CreateBy]
                                ,F.[CreateDate]
                                ,F.[UpdateBy]
                                ,F.[UpdateDate]
                                ,F.[AssetCondition]
                                ,F.[AssetCause]
                                ,F.[Company]
                                ,E.[Name]+' '+E.[LastName] [FullName]
                                ,F.CurrentApproveStep
                                ,COUNT(CCAT.Id) AS TotalApproveStep
                                ,AF.Id [IdFile]
                                ,F.[KeyNumber]
                                ,'null' [DepKeyNumber]
                                ,F.UpdateAx
                            FROM [EForm].[dbo].[TB_FixAssets] F
                            LEFT JOIN TB_RequestStatus RS ON RS.Id = F.RequestStatus
                            LEFT JOIN TB_FixAssetsApproveTrans CCAT ON CCAT.CCId = F.Id AND CCAT.Status = 'Approved By'
                            LEFT JOIN TB_User U ON U.Id = F.CreateBy
                            LEFT JOIN TB_Employee E ON E.EmployeeId = U.EmployeeId
                            LEFT JOIN TB_Company C ON C.CompanyId = F.Company
                            LEFT JOIN TB_FixAssetsAttachFile AF ON AF.CCId = F.Id
                            WHERE 
                            C.Id IN 
                            (
                            SELECT nstr FROM SplitStringList(
                            (SELECT GroupCompany FROM TB_FixAssetsCompanyGroup WHERE Id='11')
                            , ',')
                            )
                            GROUP BY 
                                F.[Id]
                                ,F.[AssetNumber]
                                ,F.[AssetDate]
                                ,F.AssetType
                                ,F.AssetCategory
                                ,F.[CurrentApproveStep]
                                ,RS.[Description]
                                ,F.[CreateBy]
                                ,F.[CreateDate]
                                ,F.[UpdateBy]
                                ,F.[UpdateDate]
                                ,F.[AssetCondition]
                                ,F.[AssetCause]
                                ,F.[Company]
                                ,E.[Name]
                                ,E.[LastName]
                                ,F.[CurrentApproveStep]
                                ,AF.[Id]
                                ,F.[KeyNumber]
                                ,F.UpdateAx
                            ORDER BY F.Id DESC  
                    ",
                    new
                    {
                        @S_AssetsCompany = S_AssetsCompany,
                        @S_AssetCategory = S_AssetCategory,
                        @S_AssetsStatus = S_AssetsStatus,
                        @S_AssetsDate = S_AssetsDate,
                        @S_AssetsNumber = S_AssetsNumber,
                        @S_AssetsCreateBy = S_AssetsCreateBy
                    }, unitOfWork.Transaction);

                    if(employee.DepartmentName=="Cost" || employee.DepartmentName=="Cost Accounting" || createBy==1){
                         dataAssets = await unitOfWork.Transaction.Connection.QueryAsync<FixAssetsGridViewModel>($@"
                            SELECT F.[Id]
                                    ,F.[AssetNumber]
                                    ,CONVERT(varchar, F.[AssetDate], 103) [AssetDate]
                                    ,CASE 
                                        WHEN F.AssetType=1 THEN 'ตัดทรัพย์สินถาวร'
                                        WHEN F.AssetType=2 THEN 'แจ้งเปลี่ยนผู้ถือครอง'
                                        WHEN F.AssetType=3 THEN 'แจ้งเปลี่ยนผู้ถือครอง & เคลื่อนย้ายอุปกรณ์'
                                    ELSE '-' END [AssetType]
                                    ,CASE 
                                        WHEN F.AssetCategory=1 THEN 'ตัดทรัพย์สินถาวร'
                                        WHEN F.AssetCategory=2 THEN 'โอนย้ายทรัพย์สิน'
                                    ELSE '-' END [AssetCategory]
                                    ,F.[CurrentApproveStep]
                                    ,RS.[Description] AS RequestStatus
                                    ,F.[CreateBy]
                                    ,F.[CreateDate]
                                    ,F.[UpdateBy]
                                    ,F.[UpdateDate]
                                    ,F.[AssetCondition]
                                    ,F.[AssetCause]
                                    ,F.[Company]
                                    ,E.[Name]+' '+E.[LastName] [FullName]
                                    ,F.CurrentApproveStep
                                    ,COUNT(CCAT.Id) AS TotalApproveStep
                                    ,AF.Id [IdFile]
                                    ,F.[KeyNumber]
                                    ,'Cost' [DepKeyNumber]
                                    ,F.UpdateAx
                                FROM [EForm].[dbo].[TB_FixAssets] F
                                LEFT JOIN TB_RequestStatus RS ON RS.Id = F.RequestStatus
                                LEFT JOIN TB_FixAssetsApproveTrans CCAT ON CCAT.CCId = F.Id AND CCAT.Status = 'Approved By'
                                LEFT JOIN TB_User U ON U.Id = F.CreateBy
                                LEFT JOIN TB_Employee E ON E.EmployeeId = U.EmployeeId
                                LEFT JOIN TB_Company C ON C.CompanyId = F.Company
                                LEFT JOIN TB_FixAssetsAttachFile AF ON AF.CCId = F.Id
                                WHERE 
                                C.Id IN 
                                (
                                SELECT nstr FROM SplitStringList(
                                (SELECT GroupCompany FROM TB_FixAssetsCompanyGroup WHERE Id='{company}')
                                , ',')
                                )
                                AND (@S_AssetsCompany IS NULL OR F.[Company] = @S_AssetsCompany)
                                AND (@S_AssetCategory IS NULL OR F.[AssetCategory] = @S_AssetCategory)
                                AND (@S_AssetsStatus IS NULL OR F.[RequestStatus] = @S_AssetsStatus)
                                AND (@S_AssetsDate IS NULL OR CONVERT(DATE,F.[AssetDate]) = @S_AssetsDate)
                                AND (@S_AssetsNumber IS NULL OR (F.[AssetNumber] LIKE '%'+ @S_AssetsNumber +'%'))
                                AND (@S_AssetsCreateBy IS NULL OR (E.[Name] LIKE '%'+ @S_AssetsCreateBy +'%'))
                                AND F.[Id] IN ({IdLine})
                                GROUP BY 
                                    F.[Id]
                                    ,F.[AssetNumber]
                                    ,F.[AssetDate]
                                    ,F.AssetType
                                    ,F.AssetCategory
                                    ,F.[CurrentApproveStep]
                                    ,RS.[Description]
                                    ,F.[CreateBy]
                                    ,F.[CreateDate]
                                    ,F.[UpdateBy]
                                    ,F.[UpdateDate]
                                    ,F.[AssetCondition]
                                    ,F.[AssetCause]
                                    ,F.[Company]
                                    ,E.[Name]
                                    ,E.[LastName]
                                    ,F.[CurrentApproveStep]
                                    ,AF.[Id]
                                    ,F.[KeyNumber]
                                    ,F.UpdateAx
                                ORDER BY F.Id DESC  
                        ", 
                        new
                        {
                            @S_AssetsCompany = S_AssetsCompany,
                            @S_AssetCategory = S_AssetCategory,
                            @S_AssetsStatus = S_AssetsStatus,
                            @S_AssetsDate = S_AssetsDate,
                            @S_AssetsNumber = S_AssetsNumber,
                            @S_AssetsCreateBy = S_AssetsCreateBy
                        }, unitOfWork.Transaction);
                    }

                    unitOfWork.Complete();
                    // return new JsonResult(dataAssets.ToList());
                    return new JsonResult(_datatableService.FormatOnce(dataAssets.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetDownload(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<AssetsAttachFileTable>(unitOfWork.Transaction);
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
        public async Task<IActionResult> OnGetCancelTransAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                    var assets = await assetsRepo.GetAsync(id);
                    assets.RequestStatus = 2;
                    assets.UpdateBy = _authService.GetClaim().UserId;
                    assets.UpdateDate = DateTime.Now;
                    await assetsRepo.UpdateAsync(assets);

                    unitOfWork.Complete();
                    AlertSuccess = "Cancel Success.";
                    return new JsonResult(new { CancelTrans = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetKeyNumberTransAsync(string number,int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    //Console.WriteLine(number+"_"+id);
                    var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                    var assets = await assetsRepo.GetAsync(id);
                    assets.KeyNumber = number;
                    assets.UpdateBy = _authService.GetClaim().UserId;
                    assets.UpdateDate = DateTime.Now;
                    await assetsRepo.UpdateAsync(assets);

                    unitOfWork.Complete();
                    AlertSuccess = "Keynumber Success.";
                    return new JsonResult(new { KeynumberTrans = "Keynumber Success." });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetCompanyMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var companyRepo = new GenericRepository<Company>(unitOfWork.Transaction);

                var companyAll = await companyRepo.GetAllAsync();

                return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.CompanyId.ToString(),
                        Text = x.CompanyId,
                    })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnGetUpdateToAxAsync(int id)
        //public IActionResult OnGetUpdateToAx(int id)
        {
            try
            {
                using (var conn = _databaseContext.GetConnection())
                {
                    conn.Open();
                    await conn.ExecuteAsync("AssetsUpdateAx",new { AssetId = id }, commandType: CommandType.StoredProcedure);
                }
                    //using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                    //{
                    //var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                    //var assets = await assetsRepo.GetAsync(id);

                    //assets.UpdateAx = 1;
                    //await assetsRepo.UpdateAsync(assets);
                    //await unitOfWork.Connection.ExecuteAsync($@"
                    //         EXEC AssetsUpdateAx {id}
                    //     ", unitOfWork);
                    //var updateax = unitOfWork.Transaction.Connection.Execute(@"
                    //           EXEC AssetsUpdateAx @pid
                    //        ",
                    //    new
                    //    {
                    //        @pid = id
                    //    },
                    //    unitOfWork.Transaction
                    //);
                    //unitOfWork.Complete();
                    AlertSuccess = "Update To AX Success.";
                    return new JsonResult(new { UpdateAx = "Update To AX Success." });
                    //return Redirect($@"/FixAssets");
                //}
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

    }
}