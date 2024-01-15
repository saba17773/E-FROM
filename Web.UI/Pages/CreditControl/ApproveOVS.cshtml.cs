using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
  public class ApproveOVSModel : PageModel
  {
    [TempData]
    public string AlertSuccess { get; set; }

    [TempData]
    public string AlertError { get; set; }
    public ViewOVSViewModel ViewOVSData { get; set; }

    [BindProperty]
    public FormCreditControlInfo_OVS FormCreditControl { get; set; }

    [BindProperty]
    public CreditControlOVSTable CreditControl_OVS { get; set; }
    public AddressTable Address_OVS { get; set; }

    [BindProperty]
    public string ApproveRemark { get; set; }

    public List<CreditControl_ApproveRemarkModel> HistoryRemark { get; set; }

    [BindProperty]
    [Required]
    public int ApproveResult { get; set; }

    public string TypeOfBusiness { get; set; }
    public string TypeOfProduct { get; set; }

    public string IsHeadQuarter { get; set; }
    public string SaleName { get; set; }
    public bool EditCreditLimit { get; set; }

    public List<CreditControlAttachFileTable> AttachFile { get; set; }

    public List<CustomerTypeTransViewModel> CustomerTypeTrans { get; set; }

    [BindProperty]
    public List<int> TypeOfCustomer { get; set; }

    private IDatabaseContext _databaseContext;
    private IDatatableService _datatableService;
    private IAuthService _authService;
    private ICreditControlService _creditControlService;
    private IEmailService _emailService;
    private IConfiguration _configuration;

    public ApproveOVSModel(
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

    private async Task InitialDataAsync(int id, int tid, string nonce)
    {
      FormCreditControl = new FormCreditControlInfo_OVS();
      CreditControl_OVS = new CreditControlOVSTable();
      Address_OVS = new AddressTable();
      AttachFile = new List<CreditControlAttachFileTable>();

      using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
      {
        var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
        var creditControlOVSRepo = new GenericRepository<CreditControlOVSTable>(unitOfWork.Transaction);
        //var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
        var addressRepo = new GenericRepository<AddressTable>(unitOfWork.Transaction);
        var typeOfBusinessRepo = new GenericRepository<CustomerTypeTable>(unitOfWork.Transaction);
        //var typeOfProductRepo = new GenericRepository<CustomerByProductTable>(unitOfWork.Transaction);
        var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
        var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

        var creditControl = await creditControlOVSRepo.GetAsync(id);
        var approveTrans = await approveTransRepo.GetAsync(tid);
        //var typeOfProduct = await typeOfProductRepo.GetAsync((int)creditControl.TypeOfProduct);

        ViewOVSData = await unitOfWork.CreditControl.GetOVSDataByCCIdAsync(id);

        var _nonce = await unitOfWork.Nonce.GetNonceByKey(nonce);

        if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
        {
          throw new Exception("รายการนี้ถูกดำเนินการไปแล้ว");
        }

        CreditControl_OVS = creditControl;

        // attach file
        var attachFile = await unitOfWork.CreditControl.GetFileByCCIdAsync(id, nameof(RequestTypeModel.OVS));

        AttachFile = attachFile.ToList();

        var customerTypeTrans = await unitOfWork.CreditControl.GetCustomerTypeTransViewByCCId(creditControl.Id);
        CustomerTypeTrans = customerTypeTrans.ToList();

        var approveFlow = await unitOfWork.CreditControl.GetApproveFlowByCCIdAsync(CreditControl_OVS.Id);
        var creditLimitEnable = approveFlow.Where(x => x.EditCreditLimit == 1);

        if (creditLimitEnable.FirstOrDefault() != null && creditLimitEnable.Any(x => x.Email == approveTrans.Email))
        {
          EditCreditLimit = true;
        }
        else
        {
          EditCreditLimit = false;
        }

        var _approveTrans = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
        var approveTransByLevel = _approveTrans.OrderBy(x => x.ApproveLevel).ToList();

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

        HistoryRemark = approveRemark;

        unitOfWork.Complete();
      }
    }

    // id = credit control id,
    // tid = approve trans id
    public async Task<IActionResult> OnGetAsync(int id, int tid, string nonce)
    {
      try
      {
        await InitialDataAsync(id, tid, nonce);

        return Page();
      }
      catch (Exception ex)
      {
        AlertError = ex.Message;
        return Redirect("/Result");
      }

    }

    public async Task<IActionResult> OnPostAsync(int id, int tid, string nonce)
    {
      try
      {
        if (ApproveResult == 0)
        {
          AlertError = "กรุณาเลือกว่าจะ อนุมัติ หรือ ไม่อนุมัติ";
          return Redirect($"/Result");
        }

        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var approveTransRepo = new GenericRepository<CreditControlApproveTransTable>(unitOfWork.Transaction);
          var creditControlOVSRepo = new GenericRepository<CreditControlOVSTable>(unitOfWork.Transaction);
          var nonceRepo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
          var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
          var approveFlowTable = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

          // check nonce
          var _nonce = await unitOfWork.Nonce.GetNonceByKey(nonce);

          if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
          {
            throw new Exception("รายการนี้ถูกดำเนินการไปแล้ว");
          }

          _nonce.IsUsed = 1;

          var approveTrans = await approveTransRepo.GetAsync(tid);
          var creditControl = await creditControlOVSRepo.GetAsync(id);
          var approveTransAll = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
          var user = await userRepo.GetAsync(creditControl.CreateBy);


          creditControl.CreditLimited = FormCreditControl.CreditLimited;
          creditControl.GuaranteeOVS_Check = FormCreditControl.GuaranteeOVS_Check;
          creditControl.GuaranteeOVS_StandbyLCAmount = FormCreditControl.GuaranteeOVS_StandbyLCAmount;
          creditControl.GuaranteeOVS_IssueDate = FormCreditControl.GuaranteeOVS_IssueDate;
          creditControl.GuaranteeOVS_ExpireDate = FormCreditControl.GuaranteeOVS_ExpireDate;
          creditControl.GuaranteeOVS_SecurityDepositAmount = FormCreditControl.GuaranteeOVS_SecurityDepositAmount;
          creditControl.GuaranteeOVS_Other = FormCreditControl.GuaranteeOVS_Other;

          creditControl.IsHeadQuarter = creditControl.IsHeadQuarter;
          creditControl.CompanyName = creditControl.CompanyName;
          creditControl.TypeOfBusiness = creditControl.TypeOfBusiness;

          // get sender email
          CreditControlApproveTransTable prevApprove = new CreditControlApproveTransTable();

          if (approveTrans.ApproveLevel == 1)
          {
            prevApprove = await approveTransRepo.GetAsync(approveTrans.ApproveLevel);
          }
          else if (approveTrans.ApproveLevel > 1)
          {
            prevApprove = await approveTransRepo.GetAsync(approveTrans.ApproveLevel - 1);
          }
          else
          {
            prevApprove.Email = "it_ea@deestone.com";
          }

          // approve?
          if (ApproveResult == 1)
          {
            approveTrans.ApproveDate = DateTime.Now;
            approveTrans.IsDone = 1;
            approveTrans.Remark = ApproveRemark;
          }
          else if (ApproveResult == 2) // reject
          {

            approveTrans.RejectDate = DateTime.Now;
            approveTrans.IsDone = 0;
            approveTrans.Remark = ApproveRemark;

            creditControl.RequestStatus = RequestStatusModel.Reject;

            if (user == null)
            {
              throw new Exception("ไม่พบ Email ของคนสร้างรายการ");
            }

            // send email
            var sendNextEmail = _emailService.SendEmail(
                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                    $@"
                                    <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                    <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                    <b>สถานะ:</b> <span class='text-danger'>ยกเลิกรายการ</span> <br/>
                                    <b>Remark:</b> {ApproveRemark}
                                ",
                    new List<string> { user.Email },
                    new List<string> { },
                    prevApprove.Email
                );

            if (sendNextEmail.Result == false)
            {
              throw new Exception(sendNextEmail.Message);
            }

            //var emailBelowApproveLevel = approveTransAll.Where(x => x.ApproveLevel < approveTrans.ApproveLevel).ToList();

            //if (emailBelowApproveLevel.Count > 0)
            //{
            //    var listEmailReject = new List<string>();

            //    foreach (var item in emailBelowApproveLevel)
            //    {
            //        listEmailReject.Add(item.Email);
            //    }

            //    // send email
            //    var sendNextEmail = _emailService.SendEmail(
            //            $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
            //            $@"
            //            <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
            //            <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
            //            <b>สถานะ:</b> <span class='text-danger'>ยกเลิกรายการ</span>
            //        ",
            //            listEmailReject,
            //            new List<string> { }
            //        );

            //    if (sendNextEmail.Result == false)
            //    {
            //        throw new Exception(sendNextEmail.Message);
            //    }
            //}
          }

          // is final approve ?
          if (creditControl.CurrentApproveStep == approveTransAll.ToList().Count && ApproveResult == 1)
          {
            creditControl.RequestStatus = RequestStatusModel.Complete;

            var emailBelowApproveLevel = approveTransAll.Where(x => x.ApproveLevel < approveTrans.ApproveLevel).ToList();

            if (emailBelowApproveLevel.Count > 0)
            {
              var listEmailNotifyComplete = new List<string>();

              foreach (var item in emailBelowApproveLevel)
              {
                listEmailNotifyComplete.Add(item.Email);
              }

              // send email
              var sendNextEmail = _emailService.SendEmail(
                      $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                      $@"
                                    <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                                    <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                                    <b>สถานะ:</b> <span class='text-success'>ดำเนินการเสร็จสิ้น</span>
                                ",
                      listEmailNotifyComplete,
                      new List<string> { },
                      approveTrans.Email
                  );

              if (sendNextEmail.Result == false)
              {
                throw new Exception(sendNextEmail.Message);
              }
            }
          }
          else if (ApproveResult == 1)
          {
            // update head table
            creditControl.CurrentApproveStep += 1;
            creditControl.RequestStatus = RequestStatusModel.WaitingForApprove;

            // next approve trans
            var nextApproveTrans = await unitOfWork.CreditControl.GetApproveTransByLevel(id, creditControl.CurrentApproveStep);

            // check if have backup email
            // var nextApproveFlow = await approveFlowTable.GetAsync(nextApproveTrans.ApproveFlowId);
            // if (nextApproveTrans.Urgent == 1)
            // {
            //     if (!string.IsNullOrEmpty(nextApproveFlow.BackupEmail))
            //     {
            //         nextApproveTrans.Email = nextApproveFlow.BackupEmail;
            //     }
            // }

            // generate nonce
            var nonceKey = Guid.NewGuid().ToString();

            await nonceRepo.InsertAsync(new NonceTable
            {
              NonceKey = nonceKey,
              CreateDate = DateTime.Now,
              ExpireDate = DateTime.Now.AddDays(7),
              IsUsed = 0
            });

            var sendNextEmail = _emailService.SendEmail(
                    $"แจ้งสถานะคำร้องขออนุมัติเพิ่มทะเบียนลูกค้ารายใหม่ เลขที่คำขอ: {creditControl.RequestNumber}",
                    $@"
                        <b>เลขที่คำขอ:</b> {creditControl.RequestNumber}<br/>
                        <b>ชื่อลูกค้า:</b> {creditControl.CompanyName}<br/>
                        <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/CreditControl/ApproveOVS?id={creditControl.Id}&tid={nextApproveTrans.Id}&nonce={nonceKey}'>คลิกที่นี่</a> <br/> 
                    ",
                    new List<string> { nextApproveTrans.Email },
                    new List<string> { },
                    prevApprove.Email
                );

            if (sendNextEmail.Result == false)
            {
              throw new Exception(sendNextEmail.Message);
            }

            nextApproveTrans.SendEmailDate = DateTime.Now;

            await approveTransRepo.UpdateAsync(nextApproveTrans);
          }

          await approveTransRepo.UpdateAsync(approveTrans);
          await creditControlOVSRepo.UpdateAsync(creditControl);
          await nonceRepo.UpdateAsync(_nonce);

          unitOfWork.Complete();

          AlertSuccess = "ดำเนินการเสร็จสิ้น";
          return Redirect($"/Result");
        }
      }
      catch (Exception ex)
      {
        AlertError = ex.Message;
        return Redirect($"/Result");
      }
    }
  }
}