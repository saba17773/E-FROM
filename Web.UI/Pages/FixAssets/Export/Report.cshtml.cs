using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.FixAssets.Export
{
    [AllowAnonymous]
    public class ReportModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public int CCId { get; set; }
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
        public string AssetsLocationFrom { get; set; }
        public string AssetsLocationTo { get; set; }
        public string txtApproveLv1 { get; set; }
        public string txtApproveLv2 { get; set; }
        public string txtApproveLv3 { get; set; }
        public string txtApproveLv4 { get; set; }
        public string txtApproveLv5 { get; set; }
        public string txtApproveLv6 { get; set; }
        public string txtApproveLv7 { get; set; }
        public string txtPositonLv1 { get; set; }
        public string txtPositonLv2 { get; set; }
        public string txtPositonLv3 { get; set; }
        public string txtPositonLv4 { get; set; }
        public string txtPositonLv5 { get; set; }
        public string txtPositonLv6 { get; set; }
        public string txtPositonLv7 { get; set; }
        public string txtRemarkLv1 { get; set; }
        public string txtRemarkLv2 { get; set; }
        public string txtRemarkLv3 { get; set; }
        public string txtRemarkLv4 { get; set; }
        public string txtRemarkLv5 { get; set; }
        public string txtRemarkLv6 { get; set; }
        public string txtRemarkLv7 { get; set; }
        public int DispositionLv1 { get; set; }
        public int DispositionLv2 { get; set; }
        public int DispositionLv3 { get; set; }
        public int DispositionLv4 { get; set; }
        public int DispositionLv5 { get; set; }
        public int DispositionLv6 { get; set; }
        public int DispositionLv7 { get; set; }
        public string AssetsNameDirecStr { get; set; }
        public List<AssetsLineTable> AssetsLine { get; set; }
        public List<AssetsAttachFileTable> AttachFile { get; set; }
        public List<AssetsLocationTable> AssetsLocationToList { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ReportModel(
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

        public async Task<IActionResult> OnGetAsync(int id)
        {

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var assetsRepo = new GenericRepository<AssetsTable>(unitOfWork.Transaction);
                var assets = await assetsRepo.GetAsync(id);
                Assets = assets;
                
                var assetsLocationRepo = new GenericRepository<AssetsLocationTable>(unitOfWork.Transaction);
                var assetsLocationAll = await assetsLocationRepo.GetAllAsync();
                var assetsLocation = assetsLocationAll.Where(x =>
                    x.Id == assets.MoveFrom)
                    .ToList();
                foreach (var item in assetsLocation)
                {
                    AssetsLocationFrom = item.Room;
                }

                var assetsLocationTo = assetsLocationAll.Where(x =>
                    x.Id == assets.MoveTo)
                    .ToList();
                foreach (var item in assetsLocationTo)
                {
                    AssetsLocationTo = item.Room;
                }

                AssetsLocationToList = assetsLocationAll.Where(x =>
                    x.Id == assets.MoveTo)
                    .ToList();

                if (assets.AssetCategory==2 && assets.MoveTo==0)
                {
                    AssetsLocationToList = assetsLocationAll.Where(x =>
                    x.Id == assets.MoveFrom)
                    .ToList();
                    //var HeadRepo = new GenericRepository<AssetsMapHeadTable>(unitOfWork.Transaction);
                    //var HeadAll = await HeadRepo.GetAllAsync();
                    //var head = HeadAll.Where(x =>
                    //    x.Director_EmpId == "80000197")
                    //.FirstOrDefault();
                    var EmployeeRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpAll = await EmployeeRepo.GetAllAsync();
                    var emp = EmpAll.Where(x =>
                        x.EmployeeId == "80000197")
                    .FirstOrDefault();
                    AssetsNameDirecStr = emp.Name + " " + emp.LastName;
                }

                var assetsUser = await unitOfWork.Assets.GetUser(assets.Id);
                AssetsNameThaiStr = assetsUser.NameThai;
                AssetsDepartmentStr = assetsUser.DepartmentName;
                
                var assetsItemRepo = new GenericRepository<AssetsLineTable>(unitOfWork.Transaction);
                var AssetsLineAll = await assetsItemRepo.GetAllAsync();
                AssetsLine = AssetsLineAll.Where(x =>
                    x.AssetsId == assets.Id)
                    .ToList();
                    
                if(assets.AssetCondition!=null){
                    txtAssetCondition = assets.AssetCondition.Replace("\n","<br>");
                }
                if(assets.AssetCause!=null){
                    txtAssetCause = assets.AssetCause.Replace("\n","<br>");
                }

                var approveTransRepo = new GenericRepository<AssetsApproveTransTable>(unitOfWork.Transaction);
                var approveTransEmail = await approveTransRepo.GetAllAsync();
                var approveTrans = approveTransEmail.Where(x =>
                    x.CCId == id)
                .ToList();
                foreach (var item in approveTrans)
                {
                    if(item.ApproveLevel==1 && item.IsDone==1){
                        txtApproveLv1 = item.Name+" "+item.LastName;
                        txtPositonLv1 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv1 = AssetsLocationTo;
                        }else{
                            DispositionLv1 = item.Disposition;
                            txtRemarkLv1 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv1 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                    if(item.ApproveLevel==2 && item.IsDone==1){
                        txtApproveLv2 = item.Name+" "+item.LastName;
                        txtPositonLv2 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv2 = AssetsLocationTo;
                        }else{
                            DispositionLv2 = item.Disposition;
                            txtRemarkLv2 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv2 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                    if(item.ApproveLevel==3 && item.IsDone==1){
                        txtApproveLv3 = item.Name+" "+item.LastName;
                        txtPositonLv3 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv3 = AssetsLocationTo;
                        }else{
                            DispositionLv3 = item.Disposition;
                            txtRemarkLv3 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv3 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                    if(item.ApproveLevel==4 && item.IsDone==1){
                        txtApproveLv4 = item.Name+" "+item.LastName;
                        txtPositonLv4 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv4 = AssetsLocationTo;
                        }else{
                            DispositionLv4 = item.Disposition;
                            txtRemarkLv4 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv4 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                    if(item.ApproveLevel==5 && item.IsDone==1){
                        txtApproveLv5 = item.Name+" "+item.LastName;
                        txtPositonLv5 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv5 = AssetsLocationTo;
                        }else{
                            DispositionLv5 = item.Disposition;
                            txtRemarkLv5 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv5 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                    if(item.ApproveLevel==6 && item.IsDone==1){
                        txtApproveLv6 = item.Name+" "+item.LastName;
                        txtPositonLv6 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv6 = AssetsLocationTo;
                        }else{
                            DispositionLv6 = item.Disposition;
                            txtRemarkLv6 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv6 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                    if(item.ApproveLevel==7 && item.IsDone==1){
                        txtApproveLv7 = item.Name+" "+item.LastName;
                        txtPositonLv7 = item.Position.Replace("\n","<br>");
                        if(assets.AssetCategory == 2){
                            txtRemarkLv7 = AssetsLocationTo;
                        }else{
                            DispositionLv7 = item.Disposition;
                            txtRemarkLv7 = item.Remark;
                            if (!String.IsNullOrEmpty(item.Remark)){
                                if(item.Remark.Length > 30){
                                    txtRemarkLv7 = item.Remark.Substring(0,30);
                                }
                            }
                        }
                    }
                }

                unitOfWork.Complete();
                return Page();
            }
        }

        //public async Task OnGetPDF(int id)
        //{
        //    await OnGetAsync(id);
        //}
        public IActionResult OnGetPDF(int id)
        {
            // string headerUrl = ($"{_configuration["Config:BaseUrl"]}/images/dsc_logo.png"); 

            var converter = new HtmlToPdf();

            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 5;
            converter.Options.MarginRight = 5;
            converter.Options.MarginTop = 5;
            converter.Options.MarginBottom = 5;
            converter.Options.DisplayFooter = true;

            PdfTextSection text = new PdfTextSection(0, 5, "DSC-ACC-FA-001 Rev.5", new System.Drawing.Font("Arial", 6));
            text.HorizontalAlign = PdfTextHorizontalAlign.Right;
            converter.Footer.Add(text);

            var doc = converter.ConvertUrl($"{_configuration["Config:BaseUrl"]}/FixAssets/Export/Report/{id}");

            var fileName = "wwwroot/files/assets/print_assets.pdf";

            doc.Save(fileName);
            doc.Close();

            var stream = new FileStream(fileName, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf");
        }

    }
}