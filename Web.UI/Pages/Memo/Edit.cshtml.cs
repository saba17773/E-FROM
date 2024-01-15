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

namespace Web.UI.Pages.Memo
{
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public MemoTable Memo { get; set; }
        public List<MemoItemTable> MemoItemList { get; set; }
        public IFormFile UploadFileMemo { get; set; }
        public List<MemoAttachFileTable> AttachFile { get; set; }
        public List<SelectListItem> MemoAttnMaster { get; set; }
        public List<SelectListItem> MemoSubjectMaster { get; set; }
        public List<SelectListItem> MemoCustomerMaster { get; set; }
        public List<SelectListItem> MemoSOMaster { get; set; }
        public List<SelectListItem> MemoQAMaster { get; set; }
        public List<SelectListItem> MemoEnquiryMaster { get; set; } 
        [BindProperty]
        public string SO { get; set; }
        [BindProperty]
        public string QA { get; set; }
        [BindProperty]
        public string EN { get; set; }
        [BindProperty]
        public string ChangeType { get; set; }
        public int CountItem { get; set; }
        [BindProperty]
        public List<string> CencelItem { get; set; }
        [BindProperty]
        public List<string> ProducedItem { get; set; }
        [BindProperty]
        public List<string> NoProducedItem { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public EditModel(
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

        private async Task InitialDataAsync(int id)
        {
            Memo = new MemoTable();
            MemoAttnMaster = await GetMemoAttnMasterAsync();
            MemoSubjectMaster = await GetMemoSubjectMasterAsync();
            MemoCustomerMaster = await GetMemoCustomerMasterAsync();
            MemoQAMaster = await GetMemoQAMasterAsync();
            MemoEnquiryMaster = await GetMemoEnquiryMasterAsync();
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var MemoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                var memo = await MemoRepo.GetAsync(id);

                Memo = memo;
                SO = memo.SO;
                QA = memo.QA;
                EN = memo.Enquiry;

                AttachFile = new List<MemoAttachFileTable>();
                var attachFile = await unitOfWork.Memo.GetFileByCCIdAsync(id);

                AttachFile = attachFile.ToList();
                
                MemoItemList = new List<MemoItemTable>();
                var memoItemList = await unitOfWork.Memo.GetItemIdAsync(id);
                
                MemoItemList = memoItemList.ToList();
                CountItem = MemoItemList.Count();

                ChangeType = "SO";
                if(CountItem>0){
                    var checktype = memoItemList.Where(x => x.MemoId == id).FirstOrDefault();
                
                    if(checktype.Enquiry != null){
                        ChangeType = "EN";
                    }else if(checktype.QA != null){
                        ChangeType = "QA";
                    }else{
                        ChangeType = "SO";
                    }
                }
                
                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await InitialDataAsync(id);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Memo");
            }

        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var memoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                    var memo = await memoRepo.GetAsync(id);
                    var memoItemRepo = new GenericRepository<MemoItemTable>(unitOfWork.Transaction);
                    
                    var CancelArr = CencelItem.ToArray();
                    var ProducedArr = ProducedItem.ToArray();
                    var NoProducedArr = NoProducedItem.ToArray();

                    memo.AttnId = Memo.AttnId;
                    memo.SubjectId = Memo.SubjectId;
                    memo.CustomerCode = Memo.CustomerCode;
                    memo.Description = Memo.Description;
                    memo.Remark = Memo.Remark;
                    memo.UpdateBy = _authService.GetClaim().UserId;
                    memo.UpdateDate = DateTime.Now;
                    memo.SO = SO;
                    memo.QA = QA;
                    memo.Enquiry = EN;
                    //Console.WriteLine(DateTime.Now);
                    await memoRepo.UpdateAsync(memo);

                    var attachFileRepo = new GenericRepository<MemoAttachFileTable>(unitOfWork.Transaction);
                    // Upload file.
                    string basePath = $"wwwroot/files/memo/{(int)id}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    var filePath = Path.GetTempFileName();
                    
                    if (UploadFileMemo != null)
                    {
                        // delete
                        await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                                DELETE FROM TB_MemoAttachFile 
                                WHERE CCId = {id}
                            ", null, unitOfWork.Transaction);
                        // upload
                        string separator = ".";
                        string TypeFile = UploadFileMemo.FileName.Substring(UploadFileMemo.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{Memo.MemoNumber + "." + TypeFile}"))
                        {
                            await UploadFileMemo.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new MemoAttachFileTable
                            {
                                CCId = (int)id,
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{Memo.MemoNumber + "." + TypeFile}"
                            });
                        }
                    }
                    //Console.WriteLine(ChangeType);
                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_MemoItem 
                        WHERE MemoId = {id}
                    ", null, unitOfWork.Transaction);

                    if(ChangeType=="SO"){
                        var dataItem = await unitOfWork.Transaction.Connection.QueryAsync($@"
                        SELECT  
                        [ITEMID] ItemId,[NAME] ItemName,[QTYORDERED] Qty,[SALESUNIT] Unit
                        FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].SALESLINE
                        WHERE SALESID = '{SO}'
                        AND DATAAREAID='dsc'
                        ", null, unitOfWork.Transaction);
                        
                        int i = 0;
                        foreach (var item in dataItem)
                        {
                            // Console.WriteLine(item.ItemId+" : Value is "+CancelArr[i]);
                            // Console.WriteLine(item.ItemId+" : Value is "+ProducedArr[i]);
                            await memoItemRepo.InsertAsync(new MemoItemTable
                            {
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                Qty = Convert.ToDouble(item.Qty),
                                Unit = item.Unit,
                                SO = SO,
                                Cancel = Convert.ToInt32(CancelArr[i]),
                                Produced = Convert.ToInt32(ProducedArr[i]),
                                NoProduced = Convert.ToInt32(NoProducedArr[i]),
                                MemoId = Convert.ToInt32(id)
                            });
                            i++;
                        }
                    }else if(ChangeType=="QA"){
                        var dataItem = await unitOfWork.Transaction.Connection.QueryAsync($@"
                        SELECT  
                        L.ITEMID [ItemId],
                        L.NAME [ItemName],
                        L.SALESQTY [Qty],
                        L.SALESUNIT [Unit],
                        L.QUOTATIONID [QuotationId]
                        FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].SalesQuotationLine L
                        WHERE L.QUOTATIONID = '{QA}'
                        AND L.DATAAREAID='dsc'
                        ", null, unitOfWork.Transaction);
                        
                        int i = 0;
                        foreach (var item in dataItem)
                        {
                            await memoItemRepo.InsertAsync(new MemoItemTable
                            {
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                Qty = Convert.ToDouble(item.Qty),
                                Unit = item.Unit,
                                QA = QA,
                                Cancel = Convert.ToInt32(CancelArr[i]),
                                Produced = Convert.ToInt32(ProducedArr[i]),
                                NoProduced = Convert.ToInt32(NoProducedArr[i]),
                                MemoId = Convert.ToInt32(id)
                            });
                            i++;
                        }
                    }else{
                        var dataItem = await unitOfWork.Transaction.Connection.QueryAsync($@"
                        SELECT 
                        [DSG_ITEMID] ItemId,[DSG_ITEMNAME] ItemName,[DSG_QTY] Qty,[DSG_UNITID] Unit
                        FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].DSG_EnquiryLine
                        WHERE DSG_ENQUIRYID = '{EN}'
                        AND DATAAREAID='dsc'
                        ", null, unitOfWork.Transaction);
                        
                        int i = 0;
                        foreach (var item in dataItem)
                        {
                            // Console.WriteLine(item.ItemId+" : Value is "+CancelArr[i]);
                            // Console.WriteLine(item.ItemId+" : Value is "+ProducedArr[i]);
                            await memoItemRepo.InsertAsync(new MemoItemTable
                            {
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                Qty = Convert.ToDouble(item.Qty),
                                Unit = item.Unit,
                                Enquiry = EN,
                                Cancel = Convert.ToInt32(CancelArr[i]),
                                Produced = Convert.ToInt32(ProducedArr[i]),
                                NoProduced = Convert.ToInt32(NoProducedArr[i]),
                                MemoId = Convert.ToInt32(id)
                            });
                            i++;
                        }
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Edit Success.";
                    return Redirect($@"/Memo");
                    // return new JsonResult(new{ NoProducedArr });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect($@"/Memo/{id}/Edit");
            }
        }

        public async Task<List<SelectListItem>> GetMemoAttnMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var memoAttnRepo = new GenericRepository<MemoAttnTable>(unitOfWork.Transaction);

                var memoAttnAll = await memoAttnRepo.GetAllAsync();

                return memoAttnAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.MemoAttn,
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetMemoSubjectMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var memoSubjectRepo = new GenericRepository<MemoSubjectTable>(unitOfWork.Transaction);

                var memoSubjectAll = await memoSubjectRepo.GetAllAsync();

                return memoSubjectAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.MemoSubject,
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetMemoCustomerMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var memoSubjectRepo = new GenericRepository<MemoCustomerTable>(unitOfWork.Transaction);

                var memoSubjectAll = await memoSubjectRepo.GetAllAsync();

                return memoSubjectAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.CustomerCode.ToString(),
                        Text = x.CustomerCode +" : "+ x.CustomerName,
                    })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnGetDeleteFileAsync(int IdFile)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<MemoAttachFileTable>(unitOfWork.Transaction);

                    var attachFile = await attachFileRepo.GetAsync(IdFile);
                    string basePath = attachFile.FilePath + "/" + attachFile.FileName;

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.File.Delete(basePath);
                        await attachFileRepo.DeleteAsync(new MemoAttachFileTable
                        {
                            Id = IdFile
                        });
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteFile = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetMemoQAMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var SOAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoCustomerTable>(@"
                    SELECT QUOTATIONID [QuatationId]
                    FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].SalesQuotationTable
                    WHERE DATAAREAID='dsc'
                    AND LEFT(QUOTATIONID,4)='QA21'
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return SOAll
                .Select(x => new SelectListItem
                {
                    Value = x.QuatationId.ToString(),
                    Text = x.QuatationId.ToString(),
                })
                .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetMemoEnquiryMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var SOAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoCustomerTable>(@"
                    SELECT [DSG_ENQUIRYID] ENQUIRY
                    FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].DSG_EnquiryLine
                    WHERE DATAAREAID='dsc'
                    AND LEFT(DSG_ENQUIRYID,4)='EN21'
                    GROUP BY DSG_ENQUIRYID
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return SOAll
                .Select(x => new SelectListItem
                {
                    Value = x.ENQUIRY.ToString(),
                    Text = x.ENQUIRY.ToString(),
                })
                .ToList();
            }
        }

        public async Task<IActionResult> OnGetEnquiryGetItemAsync(string enquiry)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoItemTransViewModel>($@"
                        SELECT 
                        [DSG_ITEMID] ItemId,[DSG_ITEMNAME] ItemName,[DSG_QTY] Qty,[DSG_UNITID] Unit
                        FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].DSG_EnquiryLine
                        WHERE DSG_ENQUIRYID = '{enquiry}'
                        AND DATAAREAID='dsc'
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

        public async Task<IActionResult> OnGetSOGetItemAsync(string so)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoItemTransViewModel>($@"
                        SELECT  
                        L.ITEMID [ItemId]
                        ,L.NAME [ItemName]
                        ,L.QTYORDERED [Qty]
                        ,L.SALESUNIT [Unit]
                        ,Q.QUOTATIONID [QuatationId]
                        ,Q.DSG_ENQUIRYID [EnquiryId]
                        FROM [frey\live].[DSL_AX40_SP1_LIVE].[dbo].SALESLINE L
                        LEFT JOIN [frey\live].[DSL_AX40_SP1_LIVE].[dbo].SalesQuotationTable Q
                        ON L.SALESID = Q.SALESIDREF AND Q.DATAAREAID='dsc'
                        WHERE L.SALESID = '{so}'
                        AND L.DATAAREAID='dsc'
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

    }

}