using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Ocsp;
using Renci.SshNet.Messages;
using Web.UI.Contexts;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;
using Web.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.ViewModels;

namespace Web.UI.Pages.Vender
{
    public class CancelModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        [StringLength(20)]
        [MaxLength(13)]
        public string VenderIDNum { get; set; }

        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string VenderCode { get; set; }

        [BindProperty]
        public string VenderCodeAX { get; set; }

        [BindProperty]
        [StringLength(150)]
        public string VenderName { get; set; }

        [BindProperty]
        [StringLength(50)]
        public string ContactName { get; set; }

        [BindProperty]
        [StringLength(20)]
        public string Telephone { get; set; }

        [BindProperty]
        [StringLength(20)]
        public string Fax { get; set; }

        [BindProperty]
        [StringLength(100)]
        public string Website { get; set; }

        [BindProperty]
        [StringLength(50)]
        public string Email { get; set; }

        [BindProperty]
        public string VenderGroup { get; set; }

        [BindProperty]
        public string VenderType { get; set; }

        [BindProperty]
        [StringLength(10)]
        public string Currency { get; set; }

        [BindProperty]
        public string ProductType { get; set; }
        [BindProperty]
        public string Paymterm { get; set; }
        [BindProperty]
        public DateTime? RequestDate { get; set; }

        [BindProperty]
        [StringLength(250)]
        public string Remark { get; set; }

        [BindProperty]
        public bool DocRef1 { get; set; }
        [BindProperty]
        public bool DocRef2 { get; set; }
        [BindProperty]
        public bool DocRef3 { get; set; }
        [BindProperty]
        public bool DocRef4 { get; set; }
        [BindProperty]
        public bool DocRef5 { get; set; }
        [BindProperty]
        public bool DocRef6 { get; set; }
        [BindProperty]
        public bool DocRef7 { get; set; }
        [BindProperty]
        public bool DocRef8 { get; set; }
        [BindProperty]
        public bool DocRef9 { get; set; }
        [BindProperty]
        public bool DocRef10 { get; set; }
        [BindProperty]
        public bool DocRef11 { get; set; }
        [BindProperty]
        public bool DocRef12 { get; set; }
        [BindProperty]
        public bool DocRef13 { get; set; }
        [BindProperty]
        public bool DocRef14 { get; set; }
        [BindProperty]
        public string DocRef15 { get; set; }
        [BindProperty]
        public bool DocRef16 { get; set; }
        [BindProperty]
        public bool DocRef17 { get; set; }
        [BindProperty]
        public bool DocRef18 { get; set; }
        [BindProperty]
        public bool DocRef19 { get; set; }
        [BindProperty]
        public bool DocRef20 { get; set; }
        [BindProperty]
        public bool DocRef21 { get; set; }
        [BindProperty]
        public bool DocRef22 { get; set; }
        [BindProperty]
        public bool DocRef23 { get; set; }
        [BindProperty]
        public string DocRef6_Desc { get; set; }
        [BindProperty]
        public string DocRef16_Desc { get; set; }
        [BindProperty]
        public string DocRef9_Desc { get; set; }

        [BindProperty]
        public string Address { get; set; }

        [BindProperty]
        public int CreateBy { get; set; }

        [BindProperty]
        public int RequestID { get; set; }
        [BindProperty]
        public string ProductTypeDetail { get; set; }
        [BindProperty]
        public string DataAreaId { get; set; }
        [BindProperty]
        public bool DataAreaId1 { get; set; }
        [BindProperty]
        public bool DataAreaId2 { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;

        public CancelModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IHelperService helperService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatableService;
            _authService = authService;
            _helperService = helperService;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(VenderPermissionModel.MANAGE_VENDER));

                RequestID = id;
                await GetData(id);

                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                var vend = await venderTableRepo.GetAsync(id);

                var vendTbTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                var vendTbTempALL = await vendTbTempRepo.GetAllAsync();
                var vendTbTemp = vendTbTempALL.Where(x => x.REQUESTID == id && x.ISCOMPLETE == 0).FirstOrDefault();

                if (vendTbTemp != null)
                {
                    if (vend.VENDCODE_AX == null) { VenderCodeAX = null; } else { VenderCodeAX = vend.VENDCODE_AX; }
                    RequestCode = vend.REQUESTCODE;
                    RequestDate = vend.REQUESTDATE;
                    VenderCode = vend.VENDCODE;
                    VenderIDNum = vend.VENDIDNUM;
                    VenderName = vend.VENDNAME;
                    if (vend.DATAAREAID == "dv") { DataAreaId1 = true; } else { DataAreaId1 = false; }
                    if (vend.DATAAREAID == "dsc") { DataAreaId2 = true; } else { DataAreaId2 = false; }
                    DataAreaId = vend.DATAAREAID;

                    Address = vendTbTemp.ADDRESS_TEMP;
                    ContactName = vendTbTemp.CONTACTNAME_TEMP;
                    Telephone = vendTbTemp.TEL_TEMP;
                    Fax = vendTbTemp.FAX_TEMP;
                    Email = vendTbTemp.EMAIL_TEMP;
                    Website = vendTbTemp.WEBSITE_TEMP;
                    Remark = vendTbTemp.REMARK_TEMP;
                    Currency = vendTbTemp.CURRENCY_TEMP;
                    ProductType = vendTbTemp.PAYMTERMID_TEMP;
                    ProductTypeDetail = vendTbTemp.PRODTYPEDETAIL_TEMP;
                    Paymterm = vendTbTemp.PAYMTERMID_TEMP;

                    var vendGroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                    var vendGroupALL = await vendGroupRepo.GetAllAsync();
                    var vendTypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                    var vendTypeALL = await vendTypeRepo.GetAllAsync();

                    VenderGroup = vendGroupALL.Where(x => x.ID == vendTbTemp.VENDGROUPID_TEMP).Select(s => s.DESCRIPTION).FirstOrDefault();
                    VenderType = vendTypeALL.Where(x => x.ID == vendTbTemp.VENDTYPEID_TEMP).Select(s => s.DESCRIPTION).FirstOrDefault();

                    var vendDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                    var vendDocALL = await vendDocRepo.GetAllAsync();

                    //checkbox
                    foreach (var docLog in vendDocALL.Where(x => x.REQUESTID == id && x.ISACTIVE == 0 && x.ISTEMP == 1))
                    {
                        if (docLog.DOCID == 1) { DocRef1 = true; }
                        if (docLog.DOCID == 2) { DocRef2 = true; }
                        if (docLog.DOCID == 3) { DocRef3 = true; }
                        if (docLog.DOCID == 4) { DocRef4 = true; }
                        if (docLog.DOCID == 5) { DocRef5 = true; }
                        if (docLog.DOCID == 6) { DocRef6 = true; DocRef6_Desc = docLog.REMARK; }
                        if (docLog.DOCID == 7) { DocRef7 = true; }
                        if (docLog.DOCID == 8) { DocRef8 = true; }
                        if (docLog.DOCID == 9) { DocRef9 = true; DocRef9_Desc = docLog.REMARK; }
                        if (docLog.DOCID == 10) { DocRef10 = true; }
                        if (docLog.DOCID == 11) { DocRef11 = true; }
                        if (docLog.DOCID == 12) { DocRef12 = true; }
                        if (docLog.DOCID == 13) { DocRef13 = true; }
                        if (docLog.DOCID == 14) { DocRef14 = true; }
                        if (docLog.DOCID == 15) { DocRef15 = docLog.REMARK; }
                        if (docLog.DOCID == 16) { DocRef16 = true; DocRef16_Desc = docLog.REMARK; }
                        if (docLog.DOCID == 17) { DocRef17 = true; }
                        if (docLog.DOCID == 18) { DocRef18 = true; }
                        if (docLog.DOCID == 19) { DocRef19 = true; }
                        if (docLog.DOCID == 20) { DocRef20 = true; }
                        if (docLog.DOCID == 21) { DocRef21 = true; }
                        if (docLog.DOCID == 22) { DocRef22 = true; }
                        if (docLog.DOCID == 23) { DocRef23 = true; }
                    }

                }
                else
                {
                    if (vend.VENDCODE_AX == null) { VenderCodeAX = "-"; } else { VenderCodeAX = vend.VENDCODE_AX; }
                    RequestCode = vend.REQUESTCODE;
                    RequestDate = vend.REQUESTDATE;
                    VenderCode = vend.VENDCODE;
                    VenderIDNum = vend.VENDIDNUM;
                    VenderName = vend.VENDNAME;
                    if (vend.DATAAREAID == "dv") { DataAreaId1 = true; } else { DataAreaId1 = false; }
                    if (vend.DATAAREAID == "dsc") { DataAreaId2 = true; } else { DataAreaId2 = false; }
                    DataAreaId = vend.DATAAREAID;
                    Address = vend.ADDRESS;
                    ContactName = vend.CONTACTNAME;
                    Telephone = vend.TEL;
                    Fax = vend.FAX;
                    Email = vend.EMAIL;
                    Website = vend.WEBSITE;
                    Remark = vend.REMARK;
                    Currency = vend.CURRENCY;
                    ProductType = vend.PRODTYPEID;
                    Paymterm = vend.PAYMTERMID;
                    ProductTypeDetail = vend.PRODTYPEDETAIL;

                    var vendGroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                    var vendGroupALL = await vendGroupRepo.GetAllAsync();
                    var vendTypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                    var vendTypeALL = await vendTypeRepo.GetAllAsync();

                    VenderGroup = vendGroupALL.Where(x => x.ID == vend.VENDGROUPID).Select(s => s.DESCRIPTION).FirstOrDefault();
                    VenderType = vendTypeALL.Where(x => x.ID == vend.VENDTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();
                    //Currency = currencyALL.Where(x => x.CODE == vend.CURRENCY).Select(s => s.DESCRIPTION).FirstOrDefault();
                    //ProductType = vendProdALL.Where(x => x.CODE == vend.PRODTYPEID).Select(s => s.CODE).FirstOrDefault();

                    var vendDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                    var vendDocALL = await vendDocRepo.GetAllAsync();
                    foreach (var docLog in vendDocALL.Where(x => x.REQUESTID == id))
                    {
                        if (docLog.DOCID == 1) { DocRef1 = true; }
                        if (docLog.DOCID == 2) { DocRef2 = true; }
                        if (docLog.DOCID == 3) { DocRef3 = true; }
                        if (docLog.DOCID == 4) { DocRef4 = true; }
                        if (docLog.DOCID == 5) { DocRef5 = true; }
                        if (docLog.DOCID == 6) { DocRef6 = true; DocRef6_Desc = docLog.REMARK; }
                        if (docLog.DOCID == 7) { DocRef7 = true; }
                        if (docLog.DOCID == 8) { DocRef8 = true; }
                        if (docLog.DOCID == 9) { DocRef9 = true; DocRef9_Desc = docLog.REMARK; }
                        if (docLog.DOCID == 10) { DocRef10 = true; }
                        if (docLog.DOCID == 11) { DocRef11 = true; }
                        if (docLog.DOCID == 12) { DocRef12 = true; }
                        if (docLog.DOCID == 13) { DocRef13 = true; }
                        if (docLog.DOCID == 14) { DocRef14 = true; }
                        if (docLog.DOCID == 15) { DocRef15 = docLog.REMARK; }
                        if (docLog.DOCID == 16) { DocRef16 = true; DocRef16_Desc = docLog.REMARK; }
                        if (docLog.DOCID == 17) { DocRef17 = true; }
                        if (docLog.DOCID == 18) { DocRef18 = true; }
                        if (docLog.DOCID == 19) { DocRef19 = true; }
                        if (docLog.DOCID == 20) { DocRef20 = true; }
                        if (docLog.DOCID == 21) { DocRef21 = true; }
                        if (docLog.DOCID == 22) { DocRef22 = true; }
                        if (docLog.DOCID == 23) { DocRef23 = true; }

                    }
                }
                    


                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                //popup question?

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var venderTable = await venderTableRepo.GetAsync(id);
                    var DatetimeNow = DateTime.Now;
                    if (venderTable.COMPLETEDATE == null)
                    {
                        if (venderTable.VENDPROCESSID != 1 && venderTable.APPROVESTATUS != 4)
                        {
                            throw new Exception("Request Vender Can't Cancel");
                        }
                        
                        // update head table
                        venderTable.APPROVESTATUS = RequestStatusModel.Cancel;
                        venderTable.COMPLETEDATE = DatetimeNow;
                        venderTable.COMPLETEBY = CreateBy;
                        await venderTableRepo.UpdateAsync(venderTable);
                    }
                    else
                    {
                        var vendTbTempRepo2 = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                        var vendTbTempALL2 = await vendTbTempRepo2.GetAllAsync();
                        var vendTbTemp2 = vendTbTempALL2.Where(x => x.REQUESTID == id && x.ISCOMPLETE == 0).Select(s => s.ID);
                        if (vendTbTemp2 != null)
                        {
                            venderTable.APPROVESTATUS = 7;
                            venderTable.UPDATEDATE = DatetimeNow;
                            venderTable.UPDATEBY = CreateBy;
                            venderTable.SENDMAILSUCCESS = 1;
                            venderTable.ISREVISE = 0;
                            await venderTableRepo.UpdateAsync(venderTable);

                            foreach (var vendTb in vendTbTemp2)
                            {
                                var vendTbTempUpdate = await vendTbTempRepo2.GetAsync(vendTb);
                                vendTbTempUpdate.ISCOMPLETE = 2;
                                await vendTbTempRepo2.UpdateAsync(vendTbTempUpdate);
                            }

                            var LogDocRepo2 = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                            var LogDocALL2 = await LogDocRepo2.GetAllAsync();
                            var LogDocFilters2 = LogDocALL2.Where(x => x.REQUESTID == id && x.ISACTIVE == 0 && x.ISTEMP == 1).Select(s => s.ID);
                            foreach (var LogDocFilter2 in LogDocFilters2)
                            {
                                var LogDoc2 = await LogDocRepo2.GetAsync(LogDocFilter2);
                                LogDoc2.ISACTIVE = 2;
                                LogDoc2.ISTEMP = 0;

                                await LogDocRepo2.UpdateAsync(LogDoc2);
                            }

                            var LogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                            var LogFileAll = await LogFileRepo.GetAllAsync();
                            var LogFileFilters = LogFileAll.Where(x => x.REQUESTID == id && x.ISACTIVE == 0 && x.ISTEMP == 1).Select(s => s.ID);
                            foreach (var LogFileFilter in LogFileFilters)
                            {
                                var LogFile = await LogFileRepo.GetAsync(LogFileFilter);
                                LogFile.ISACTIVE = 2;
                                LogFile.ISTEMP = 0;

                                await LogFileRepo.UpdateAsync(LogFile);
                            }
                        }
                        else
                        {
                            venderTable.APPROVESTATUS = 7;
                            venderTable.UPDATEDATE = DatetimeNow;
                            venderTable.UPDATEBY = CreateBy;
                            venderTable.SENDMAILSUCCESS = 1;
                            venderTable.ISREVISE = 0;
                            await venderTableRepo.UpdateAsync(venderTable);
                        }

                    }
                    unitOfWork.Complete();

                    AlertSuccess = "ยกเลิก Request เสร็จสิ้น";
                    return Redirect($"/");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/");
            }
        }

        public async Task<IActionResult> OnPostGridAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LogFileGridViewModel>($@"
                        SELECT ID,
		                    REQUESTID,
		                    FILENAME,
		                    UPLOADDATE,
		                    ISACTIVE,
		                    CREATEBY
                    FROM TB_VenderLogFile 
                    WHERE (REQUESTID = {id} AND ISACTIVE = 1) OR (REQUESTID = {id} AND ISTEMP = 1)
                    ", null, unitOfWork.Transaction);
                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
