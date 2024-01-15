using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class DashboardModel : PageModel
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

        public DashboardModel(
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
        public IActionResult OnGet(string plant)
        {
            try
            {
                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetDocAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO" &&
                        x.CHECKOUTBY == 0 &&
                        Convert.ToDateTime(x.WEIGHTOUTDATE).ToString("d") == Convert.ToDateTime(DateTime.Now).ToString("d") && 
                        x.CHECKOUTDATE == null) 
                        .OrderBy(x => x.NO)
                        .ToList();
                    
                    unitOfWork.Complete();
                    
                    return new JsonResult(QingTrans.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetCheckOutAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO" &&
                        x.WEIGHTOUT > 0 &&
                        x.CHECKOUTDATE == null &&
                        Convert.ToDateTime(x.WEIGHTOUTDATE).ToString("d") == Convert.ToDateTime(DateTime.Now).ToString("d"))
                        .OrderBy(x => x.NO)
                        .ToList();
                    
                    unitOfWork.Complete();
                    
                    return new JsonResult(QingTrans.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetBayInAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO" &&
                        x.BAYID == 0 &&
                        Convert.ToDateTime(x.CHECKINDATE).ToString("d") == Convert.ToDateTime(DateTime.Now).ToString("d"))
                        .OrderBy(x => x.NO)
                        .ToList();
                    
                    unitOfWork.Complete();
                    
                    return new JsonResult(QingTrans.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        public  async Task<IActionResult> OnGetBayOutAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var qingRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var qing = await qingRepo.GetAllAsync();
                    QingTrans = qing.Where(x =>
                        x.PLANT == "SVO" &&
                        x.BAYID != 0 &&
                        Convert.ToDateTime(x.ASSIGNBAYDATE).ToString("d") == Convert.ToDateTime(DateTime.Now).ToString("d"))
                        .OrderBy(x => x.NO)
                        .ToList();
                    
                    unitOfWork.Complete();
                    
                    return new JsonResult(QingTrans.ToList());
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }
    }
}
