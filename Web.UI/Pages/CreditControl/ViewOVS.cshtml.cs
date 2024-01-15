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
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.CreditControl;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Infrastructure.ViewModels.CreditControl;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl
{
  public class ViewOVSModel : PageModel
  {
    public ViewOVSViewModel ViewOVSData { get; set; }

    public List<CreditControlAttachFileTable> AttachFile { get; set; }

    public List<CreditControl_ApproveRemarkModel> ApproveRemark { get; set; }

    public List<CustomerTypeTransViewModel> CustomerTypeTrans { get; set; }

    private IDatabaseContext _databaseContext;
    private IDatatableService _datatableService;
    private IAuthService _authService;
    private ICreditControlService _creditControlService;
    private IEmailService _emailService;
    private IConfiguration _configuration;

    public ViewOVSModel(
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
      AttachFile = new List<CreditControlAttachFileTable>();

      using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
      {
        var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
        var creditControlOVSRepo = new GenericRepository<CreditControlOVSTable>(unitOfWork.Transaction);
        var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

        // fetch view ovs data
        ViewOVSData = await unitOfWork.CreditControl.GetOVSDataByCCIdAsync(id);

        var creditControl = await creditControlOVSRepo.GetAsync(id);

        // attach file
        var attachFile = await unitOfWork.CreditControl.GetFileByCCIdAsync(id, nameof(RequestTypeModel.OVS));

        AttachFile = attachFile.ToList();

        var approveTrans = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
        var approveTransByLevel = approveTrans.OrderBy(x => x.ApproveLevel).ToList();

        List<CreditControl_ApproveRemarkModel> approveRemark = new List<CreditControl_ApproveRemarkModel>();

        foreach (var item in approveTransByLevel)
        {
          var approveName = await approveFlowRepo.GetAsync(item.ApproveFlowId);
          approveRemark.Add(new CreditControl_ApproveRemarkModel
          {
            Remark = item.Remark,
            Name = approveName?.Name ?? ""
          });
        }

        ApproveRemark = approveRemark;

        var customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(creditControl.Id);
        CustomerTypeTrans = customerTypeTrans.ToList();

        unitOfWork.Complete();
      }
    }

    public async Task OnGet(int id)
    {
      await InitialDataAsync(id);
    }
  }
}