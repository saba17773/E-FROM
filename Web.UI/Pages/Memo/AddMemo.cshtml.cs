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
using System.Diagnostics;

namespace Web.UI.Pages.Memo
{
    public class AddMemoModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public List<string> CencelItem { get; set; }
        [BindProperty]
        public List<string> ProducedItem { get; set; }
        [BindProperty]
        public List<string> NoProducedItem { get; set; }
        [BindProperty]
        public string ChangeType { get; set; }

        [BindProperty]
        public MemoTable Memo { get; set; }
        public MemoItemTable MemoItem { get; set; }
        public MemoAttnTable Attn { get; set; }
        public MemoSubjectTable Subject { get; set; }
        public IFormFile UploadFileMemo { get; set; }
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
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddMemoModel(
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
            Memo = new MemoTable();
            MemoItem = new MemoItemTable();
            Attn = new MemoAttnTable();
            Subject = new MemoSubjectTable();
            MemoAttnMaster = await GetMemoAttnMasterAsync();
            MemoSubjectMaster = await GetMemoSubjectMasterAsync();
            MemoCustomerMaster = await GetMemoCustomerMasterAsync();
            MemoEnquiryMaster = await GetMemoEnquiryMasterAsync();
        }

        public async Task OnGetAsync()
        {
            await InitialDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(MemoPermissionModel.ADD_MEMO));
                if (!ModelState.IsValid)
                {
                    await InitialDataAsync();

                    return Page();
                }
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var memoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                    var memoItemRepo = new GenericRepository<MemoItemTable>(unitOfWork.Transaction);
                    var attachFileRepo = new GenericRepository<MemoAttachFileTable>(unitOfWork.Transaction);
                    var numberSeqRepo = new GenericRepository<NumberSeqTable>(unitOfWork.Transaction);
                    var numberSeq = await numberSeqRepo.GetAllAsync();
                    var numberSeqMemo = numberSeq.Where(x => x.SeqKey == "Memo").FirstOrDefault();
                    numberSeqMemo.SeqValue += 1;
                    await numberSeqRepo.UpdateAsync(numberSeqMemo);
                    
                    var reqNumber = _creditControlService.GetLatestRequestNumberMemoAsync(numberSeqMemo.SeqValue.ToString(), "Memo");
                    // Console.WriteLine(ChangeType);
                    var CancelArr = CencelItem.ToArray();
                    var ProducedArr = ProducedItem.ToArray();
                    var NoProducedArr = NoProducedItem.ToArray();
                    //Console.WriteLine(string.Join(",", CencelItem.ToArray()));
                    
                    var newMemo = new MemoTable
                    {
                        MemoNumber = reqNumber,
                        MemoDate = DateTime.Now,
                        AttnId = Memo.AttnId,
                        SubjectId = Memo.SubjectId,
                        CustomerCode = Memo.CustomerCode,
                        Description = Memo.Description,
                        Remark = Memo.Remark,
                        SO = SO,
                        QA = QA,
                        Enquiry = EN,
                        CreateBy = _authService.GetClaim().UserId,
                        CreateDate = DateTime.Now
                    };

                    var addMemo = await memoRepo.InsertAsync(newMemo);

                    // Upload file.
                    string basePath = $"wwwroot/files/memo/{(int)addMemo}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    var filePath = Path.GetTempFileName();
                    if (UploadFileMemo != null)
                    {
                        string separator = ".";
                        string TypeFile = UploadFileMemo.FileName.Substring(UploadFileMemo.FileName.IndexOf(separator) + separator.Length);

                        using (var stream = System.IO.File.Create($"{basePath}/{reqNumber + "." + TypeFile}"))
                        {
                            await UploadFileMemo.CopyToAsync(stream);

                            await attachFileRepo.InsertAsync(new MemoAttachFileTable
                            {
                                CCId = (int)addMemo,
                                FileNo = 1,
                                FilePath = $"{basePath}",
                                FileName = $"{reqNumber + "." + TypeFile}"
                            });
                        }
                    }

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
                                MemoId = Convert.ToInt32(addMemo)
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
                                MemoId = Convert.ToInt32(addMemo)
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
                                MemoId = Convert.ToInt32(addMemo)
                            });
                            i++;
                        }
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Add Memo Success. MemoNumber : "+reqNumber;
                    return Redirect("/Memo");
                    // return new JsonResult(new{ ProducedArr });
                }

            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Memo/AddMemo");
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

        public async Task<IActionResult> OnGetAddAttnAsync(string attn_name)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attnRepo = new GenericRepository<MemoAttnTable>(unitOfWork.Transaction);
                    var newAttn = new MemoAttnTable
                    {
                        MemoAttn = attn_name
                    };

                    var addAttn = await attnRepo.InsertAsync(newAttn);

                    unitOfWork.Complete();
                    AlertSuccess = "Add Attn Success.";
                    return new JsonResult(new { AddAttn = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetAddSubjectAsync(string subject_name)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var subjectRepo = new GenericRepository<MemoSubjectTable>(unitOfWork.Transaction);
                    var newSubject = new MemoSubjectTable
                    {
                        MemoSubject = subject_name
                    };

                    var addSubject = await subjectRepo.InsertAsync(newSubject);

                    unitOfWork.Complete();
                    AlertSuccess = "Add Subject Success.";
                    return new JsonResult(new { AddSubject = 1 });
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
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
            {
                var SOAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoCustomerTable>(@"
                    SELECT QUOTATIONID [QuatationId]
                    FROM SalesQuotationTable
                    WHERE DATAAREAID='dsc'
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
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
            {
                var SOAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoCustomerTable>(@"
                    SELECT [DSG_ENQUIRYID] ENQUIRY
                    FROM DSG_EnquiryLine
                    WHERE DATAAREAID='dsc'
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

        //GET ITEM BY EN
        public async Task<IActionResult> OnGetEnquiryGetItemAsync(string enquiry)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoItemTransViewModel>($@"
                        SELECT 
                        [DSG_ITEMID] ItemId,[DSG_ITEMNAME] ItemName,[DSG_QTY] Qty,[DSG_UNITID] Unit
                        FROM DSG_EnquiryLine
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

        //GET ITEM BY SO
        public async Task<IActionResult> OnGetSOGetItemAsync(string so)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoItemTransViewModel>($@"
                        SELECT  
                        L.ITEMID [ItemId]
                        ,L.NAME [ItemName]
                        ,L.QTYORDERED [Qty]
                        ,L.SALESUNIT [Unit]
                        ,Q.QUOTATIONID [QuotationId]
                        ,Q.DSG_ENQUIRYID [EnquiryId]
                        FROM SALESLINE L
                        LEFT JOIN SalesQuotationTable Q
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

        //GET ITEM BY QA
        public async Task<IActionResult> OnGetQAGetItemAsync(string qa)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    
                    var ItemAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoItemTransViewModel>($@"
                        SELECT  
                        L.ITEMID [ItemId],
                        L.NAME [ItemName],
                        L.SALESQTY [Qty],
                        L.SALESUNIT [Unit],
                        L.QUOTATIONID [QuotationId]
                        FROM SalesQuotationLine L
                        WHERE L.QUOTATIONID = '{qa}'
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
        
        //GET EN BY SO
        public async Task<IActionResult> OnGetEnquiryBySOAsync(string salesid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    
                    var EN = await unitOfWork.Transaction.Connection.QueryAsync<MemoENTable>($@"
                        SELECT TOP 1 Dsg_EnquiryId
                        FROM SalesQuotationTable
                        WHERE SalesIdRef = '{salesid}'
                        AND DATAAREAID = 'dsc'
                    ", null, unitOfWork.Transaction);
                    
                    unitOfWork.Complete();

                    return new JsonResult(EN.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }
        //SO
        public async Task<JsonResult> OnPostSOGridAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
            {
                var employeeRepo = new GenericRepository<MemoSOTable>(unitOfWork.Transaction);

                var field = new
                {
                    salesId = "SalesId"
                };

                var filter = _datatableService.Filter(Request, field);

                var soAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoSOTable>(@"
                    SELECT TOP 500 SalesId,QuotationId
                    FROM SALESTABLE
                    WHERE " + filter + @"  
                    AND DATAAREAID='dsc'
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatableService.Format(Request, soAll.ToList()));
            }
        }

        //QA
        public async Task<JsonResult> OnPostQAGridAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
            {
                var employeeRepo = new GenericRepository<MemoQATable>(unitOfWork.Transaction);

                var field = new
                {
                    quotationId = "QuotationId"
                };

                var filter = _datatableService.Filter(Request, field);

                var soAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoQATable>(@"
                    SELECT TOP 500 QuotationId
                    FROM SalesQuotationTable
                    WHERE " + filter + @"  
                    AND DATAAREAID='dsc'
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatableService.Format(Request, soAll.ToList()));
            }
        }

        //EN
        public async Task<JsonResult> OnPostENGridAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
            {
                var employeeRepo = new GenericRepository<MemoENTable>(unitOfWork.Transaction);

                var field = new
                {
                    dsg_EnquiryId = "Dsg_EnquiryId"
                };

                var filter = _datatableService.Filter(Request, field);

                var soAll = await unitOfWork.Transaction.Connection.QueryAsync<MemoENTable>(@"
                    SELECT TOP 500 Dsg_EnquiryId
                    FROM DSG_EnquiryLine
                    WHERE " + filter + @"  
                    AND DATAAREAID='dsc'
                    GROUP BY Dsg_EnquiryId
                    ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatableService.Format(Request, soAll.ToList()));
            }
        }

    }
}