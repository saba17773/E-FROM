using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Dashboard
{
    public class WeighInOutModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        public List<QingOVS_TB> QingTrans { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public WeighInOutModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }
        public  async Task<IActionResult> OnGetAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO")
                        .ToList();
                    // foreach (var item in QingTrans)
                    // {
                        // Console.WriteLine(item.ID);
                    // }
                }
                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetInAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO" &&
                        x.STATUS == 1 &&
                        Convert.ToDateTime(x.CHECKINDATE).ToString("d") == Convert.ToDateTime(DateTime.Now).ToString("d"))
                        .OrderBy(x => x.NO)
                        .ToList();
                    unitOfWork.Complete();

                    return new JsonResult(QingTrans.ToList());
                }
                // return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetOutAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO" &&
                        x.STATUS == 2 &&
                        Convert.ToDateTime(x.ASSIGNBAYDATE).ToString("d") == Convert.ToDateTime(DateTime.Now).ToString("d") && 
                        x.WEIGHTOUTDATE == null)
                        .OrderBy(x => x.NO)
                        .ToList();
                    // Console.WriteLine(Convert.ToDateTime(DateTime.Now).ToString("d"));
                    // foreach (var item in QingTrans)
                    // {
                    //    Console.WriteLine(item.ID);
                    // }
                    unitOfWork.Complete();
                    
                    return new JsonResult(QingTrans.ToList());
                }
                // return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }
        
    }
}
