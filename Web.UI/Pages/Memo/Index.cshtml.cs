using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Memo
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public MemoTable Memo { get; set; }
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
                await _authService.CanAccess(nameof(MemoPermissionModel.VIEW_MEMO));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Memo");
            }
        }

        public async Task<IActionResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<MemoGridViewModel>(@"
                        SELECT 
                        M.Id,
                        M.MemoNumber,
                        CONVERT(varchar, M.MemoDate, 103) AS MemoDate,
                        A.MemoAttn,
                        S.MemoSubject,
                        M.CustomerCode,
                        C.CustomerName,
                        M.Description,
                        M.Remark
                        FROM TB_Memo M
                        LEFT JOIN TB_MemoAttn A ON M.AttnId = A.Id
                        LEFT JOIN TB_MemoSubject S ON M.SubjectId = S.Id
                        JOIN 
                        (
                            SELECT CustomerCode,CustomerName
                            FROM TB_MemoCustomer
                            GROUP BY CustomerCode,CustomerName
                        )C
                        ON C.CustomerCode = M.CustomerCode
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

        public async Task<IActionResult> OnGetDownload(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<MemoAttachFileTable>(unitOfWork.Transaction);
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

        public async Task<IActionResult> OnGetDeleteSubjectAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var memoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                    var memo = await memoRepo.GetAllAsync();
                    var MemoTrans = memo.Where(x =>
                        x.SubjectId == id)
                        .ToList();
                    // Console.WriteLine(MemoTrans.Count);
                    if (MemoTrans.Count > 0)
                    {
                        AlertError = "Subject Is Use.";
                        return new JsonResult(new { Delete = 0 });
                    }else{
                        var subjectRepo = new GenericRepository<MemoSubjectTable>(unitOfWork.Transaction);

                        await subjectRepo.DeleteAsync(new MemoSubjectTable
                        {
                            Id = id
                        });

                        unitOfWork.Complete();
                        AlertSuccess = "Delete Subject Success.";
                        return new JsonResult(new { Delete = 1 });
                    }
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteAttnAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var memoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                    var memo = await memoRepo.GetAllAsync();
                    var MemoTrans = memo.Where(x =>
                        x.AttnId == id)
                        .ToList();
                    
                    if (MemoTrans.Count > 0)
                    {
                        AlertError = "Attn Is Use.";
                        return new JsonResult(new { Delete = 0 });
                    }else{
                        var attnRepo = new GenericRepository<MemoAttnTable>(unitOfWork.Transaction);

                        await attnRepo.DeleteAsync(new MemoAttnTable
                        {
                            Id = id
                        });

                        unitOfWork.Complete();
                        AlertSuccess = "Delete Attn Success.";
                        return new JsonResult(new { Delete = 1 });
                    }
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