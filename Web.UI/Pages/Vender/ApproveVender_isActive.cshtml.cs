using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Vender
{
    public class ApproveVender_isActiveModel : PageModel
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
        public int isActive { get; set; }

        [BindProperty]
        public int RequestID { get; set; }

        [BindProperty]
        public string ApproveRemark { get; set; }

        [BindProperty]
        [Required]
        public int ApproveResult { get; set; }

        [BindProperty]
        public string ProductTypeDetail { get; set; }
        [BindProperty]
        public string DataAreaId { get; set; }
        [BindProperty]
        public bool DataAreaId1 { get; set; }
        [BindProperty]
        public bool DataAreaId2 { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ApproveVender_isActiveModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int VendID, int TranID, string nonce)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    

                    var _nonce = await unitOfWork.VenderNonce.GetNonceByKey(nonce);
                    //if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
                    if (_nonce.IsUsed == 1)
                    {
                        AlertError = "Link Is Used.";
                        return Redirect($"/");
                    }

                    RequestID = VendID;
                    await GetData(VendID);

                    /*var vendTbTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                    var vendTbTempALL = await vendTbTempRepo.GetAllAsync();
                    var vendTbTemp = vendTbTempALL.Where(x => x.REQUESTID == VendID && x.ISCOMPLETE == 0).FirstOrDefault();
                    if (vendTbTemp != null)
                    {
                        var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                        var Vender = await venderTableRepo.GetAsync(VendID);

                        var vengroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                        var ventypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                        //var prodtypeRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);

                        var VenderGroup_Desc = await vengroupRepo.GetAsync(vendTbTemp.VENDGROUPID_TEMP);
                        var VenderType_Desc = await ventypeRepo.GetAsync(vendTbTemp.VENDTYPEID_TEMP);
                        //var ProdType_Desc = await prodtypeRepo.GetAllAsync();

                        VenderIDNum = Vender.VENDIDNUM;
                        VenderName = Vender.VENDNAME;
                        VenderGroup = VenderGroup_Desc.DESCRIPTION;
                        VenderType = VenderType_Desc.DESCRIPTION;
                        ProductType = vendTbTemp.PRODTYPEID_TEMP;
                    }

                    else
                    {
                        var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                        var vengroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                        var ventypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                        //var prodtypeRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);

                        var Vender = await venderTableRepo.GetAsync(VendID);
                        var VenderGroup_Desc = await vengroupRepo.GetAsync(Vender.VENDGROUPID);
                        var VenderType_Desc = await ventypeRepo.GetAsync(Vender.VENDTYPEID);
                        //var ProdType_Desc = await prodtypeRepo.GetAllAsync();

                        VenderIDNum = Vender.VENDIDNUM;
                        VenderName = Vender.VENDNAME;
                        VenderGroup = VenderGroup_Desc.DESCRIPTION;
                        VenderType = VenderType_Desc.DESCRIPTION;
                        ProductType = Vender.PRODTYPEID;
                        //ProductType = ProdType_Desc.Where(x => x.CODE == Vender.PRODTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();
                    }


                    unitOfWork.Complete();*/

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync(int VendID, int TranID, string nonce, string email)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "��س����͡��Ҩ� ͹��ѵ� ���� ���͹��ѵ�";
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {

                    AlertError = "��س�������˵ط�����͹��ѵ� !!";
                    return Redirect($"/Vender/ApproveVender_isActive?VendID={VendID}&TranID={TranID}&nonce={nonce}&email={email}");
                }
                if (!ModelState.IsValid)
                {
                    return Redirect($"/Vender/ApproveVender_isActive?VendID={VendID}&TranID={TranID}&nonce={nonce}&email={email}");
                }



                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveTransRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);

                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var venderTable = await venderTableRepo.GetAsync(VendID);

                    var DatetimeNow = DateTime.Now;

                    int approvemasterid = venderTable.APPROVEMASTERID;
                    var _nonce = await unitOfWork.VenderNonce.GetNonceByKey(nonce);
                    if (_nonce.IsUsed == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }

                    _nonce.IsUsed = 1;

                    var approveTrans = await approveTransRepo.GetAsync(TranID);
                    var approvelevel = approveTrans.APPROVELEVEL;

                    var approveTransRepo2 = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    var approveTranAllLevel = await approveTransRepo2.GetAllAsync();
                    var approveTranLevel = approveTranAllLevel.Where(x => x.REQUESTID == VendID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == approvelevel &&
                                                                    x.ISCURRENTAPPROVE == 1);
                    foreach (var approveTransUpdate in approveTranLevel)
                    {
                        // update approve trans
                        approveTransUpdate.ISDONE = 1;

                        if (approveTransUpdate.EMAIL == email)
                        {
                            approveTransUpdate.REMARK = ApproveRemark;

                            if (ApproveResult == 1)
                            {
                                approveTransUpdate.APPROVEDATE = DatetimeNow;
                            }
                            else if (ApproveResult == 2)
                            {

                                approveTransUpdate.REJECTDATE = DatetimeNow;
                            }
                        }

                        await approveTransRepo2.UpdateAsync(approveTransUpdate);
                    }
                   
                    var approveTransAll = await unitOfWork.VenderControl.GetTotalLevelInApproveTrans(VendID, approvemasterid);

                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(venderTable.CREATEBY);
                    var EmployeeId = UserALL.EmployeeId;
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                    var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                    var EmpFullName = EmpFName + " " + EmpLName;

                    // is final approve ?
                    if (venderTable.CURRENTAPPROVESTEP == approveTransAll.ToList().Count && ApproveResult == 1)
                    {

                        var vendTbTempRepo3 = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                        var vendTbTempALL3 = await vendTbTempRepo3.GetAllAsync();
                        var vendTbTemp3 = vendTbTempALL3.Where(x => x.REQUESTID == VendID && x.ISCOMPLETE == 0).FirstOrDefault();

                        if (vendTbTemp3 != null)
                        {
                            int isactive = 0;
                            var isactivesub = "";
                            if (venderTable.ISACTIVE == 0)
                            {
                                isactive = 2;
                                isactivesub = "Inactive";
                            }
                            else
                            {
                                isactive = 0;
                                isactivesub = "Active";
                            }

                            var vendTbTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                            var vendTbTempALL = await vendTbTempRepo.GetAllAsync();
                            var vendTbTemp = vendTbTempALL.Where(x => x.ISCOMPLETE == 0 && x.REQUESTID == VendID).FirstOrDefault();
                            var vendcodeax = venderTable.VENDCODE_AX;

                            venderTable.APPROVESTATUS = RequestStatusModel.Successfully;
                            venderTable.ISREVISE = 0;
                            venderTable.SENDMAILSUCCESS = 1;
                            venderTable.UPDATEDATE = DatetimeNow;
                            venderTable.ADDRESS = vendTbTemp.ADDRESS_TEMP;
                            venderTable.CONTACTNAME = vendTbTemp.CONTACTNAME_TEMP;
                            venderTable.TEL = vendTbTemp.TEL_TEMP;
                            venderTable.FAX = vendTbTemp.FAX_TEMP;
                            venderTable.WEBSITE = vendTbTemp.WEBSITE_TEMP;
                            venderTable.EMAIL = vendTbTemp.EMAIL_TEMP;
                            venderTable.VENDGROUPID = vendTbTemp.VENDGROUPID_TEMP;
                            venderTable.VENDTYPEID = vendTbTemp.VENDTYPEID_TEMP;
                            venderTable.CURRENCY = vendTbTemp.CURRENCY_TEMP;
                            venderTable.PRODTYPEID = vendTbTemp.PRODTYPEID_TEMP;
                            venderTable.PAYMTERMID = vendTbTemp.PAYMTERMID_TEMP;
                            venderTable.REMARK = vendTbTemp.REMARK_TEMP;
                            venderTable.ISACTIVE = isactive;
                            
                            await venderTableRepo.UpdateAsync(venderTable);

                            var vendTbTempRepo2 = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                            var vendTbTempALL2 = await vendTbTempRepo2.GetAllAsync();
                            var vendTbTemp2 = vendTbTempALL2.Where(x => x.REQUESTID == VendID && x.ISCOMPLETE == 0).Select(s => s.ID);
                            if (vendTbTemp2 != null)
                            {
                                foreach (var vendTb in vendTbTemp2)
                                {
                                    var vendTbTempUpdate = await vendTbTempRepo2.GetAsync(vendTb);
                                    vendTbTempUpdate.ISCOMPLETE = 1;
                                    await vendTbTempRepo2.UpdateAsync(vendTbTempUpdate);
                                }
                            }

                            //update LogDoc old
                            var LogDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                            var LogDocALL = await LogDocRepo.GetAllAsync();
                            var LogDocFilters = LogDocALL.Where(x => x.REQUESTID == VendID && x.ISACTIVE == 1 && x.ISTEMP == 0).Select(s => s.ID);
                            foreach (var LogDocFilter in LogDocFilters)
                            {
                                var LogDoc = await LogDocRepo.GetAsync(LogDocFilter);
                                LogDoc.ISACTIVE = 0;

                                await LogDocRepo.UpdateAsync(LogDoc);
                            }

                            //update LogDoc new
                            var LogDocRepo2 = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                            var LogDocALL2 = await LogDocRepo2.GetAllAsync();
                            var LogDocFilters2 = LogDocALL2.Where(x => x.REQUESTID == VendID && x.ISACTIVE == 0 && x.ISTEMP == 1).Select(s => s.ID);
                            foreach (var LogDocFilter2 in LogDocFilters2)
                            {
                                var LogDoc2 = await LogDocRepo2.GetAsync(LogDocFilter2);
                                LogDoc2.ISACTIVE = 1;
                                LogDoc2.ISTEMP = 0;

                                await LogDocRepo2.UpdateAsync(LogDoc2);
                            }

                            // update file new
                            var LogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                            var LogFileAll = await LogFileRepo.GetAllAsync();
                            var LogFileFilters = LogFileAll.Where(x => x.REQUESTID == VendID && x.ISACTIVE == 0 && x.ISTEMP == 1).Select(s => s.ID);
                            foreach (var LogFileFilter in LogFileFilters)
                            {
                                var LogFile = await LogFileRepo.GetAsync(LogFileFilter);
                                LogFile.ISACTIVE = 1;
                                LogFile.ISTEMP = 0;

                                await LogFileRepo.UpdateAsync(LogFile);
                            }

                            var emailcomplete = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                            /*foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1 && x.IsActive == 1))
                            {
                                emailcomplete.Add(emaillog.Email);
                            }*/
                            emailcomplete.Add(UserALL.Email);


                            var sendCompleteEmail = _emailService.SendEmail(
                                     $"Subject : {venderTable.REQUESTCODE} / {venderTable.VENDCODE_AX} / ���Թ��� {isactivesub} ���º��������",
                                    $@"
                                     <b>���Թ��� {isactivesub} ���º��������</b> <br/><br/>
                                     <b>�Ţ���Ӣ�   {venderTable.REQUESTCODE} </b> <br/>
                                     <b>���ʼ����    {venderTable.VENDCODE_AX} </b> <br/>
                                     <b>���ͼ����    {venderTable.VENDNAME} </b> <br/> <br/><br/><br/>
                                 ",
                                    emailcomplete,
                                    new List<string> { }
                                );

                            if (sendCompleteEmail.Result == false)
                            {
                                throw new Exception(sendCompleteEmail.Message);
                            }
                            var vendGroup = "";
                            if (venderTable.VENDTYPEID == 0 || venderTable.VENDTYPEID == 2)
                            {
                                vendGroup = "L-AP1";
                            }
                            else if (venderTable.VENDTYPEID == 1)
                            {
                                vendGroup = "O-AP";
                            }
                            else
                            {
                                vendGroup = "OTHL-AP";
                            }
                            var connString = "AXCust";
                            using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
                            {
                                var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE VENDTABLE
                                        SET STREET = @STREET,
                                            PHONE = @PHONE,
                                            TELEFAX = @TELEFAX,
                                            URL = @URL,
                                            EMAIL = @EMAIL,
                                            CURRENCY = @CURRENCY,
                                            ITEMBUYERGROUPID = @ITEMBUYERGROUPID,
                                            PAYMTERMID = @PAYMTERMID,
                                            BLOCKED = @BLOCKED,
                                            TAXWITHHOLDVENDORTYPE_TH = @TAXWITHHOLDVENDORTYPE_TH,
                                            VENDGROUP = @VENDGROUP
                                    WHERE ACCOUNTNUM =  @ACCOUNTNUM AND DATAAREAID = @DATAAREAID",
                                    new
                                    {
                                        @STREET = venderTable.ADDRESS,
                                        @PHONE = venderTable.TEL,
                                        @TELEFAX = venderTable.FAX,
                                        @URL = venderTable.WEBSITE,
                                        @EMAIL = venderTable.EMAIL,
                                        @CURRENCY = venderTable.CURRENCY,
                                        @ITEMBUYERGROUPID = venderTable.PRODTYPEID,
                                        @PAYMTERMID = venderTable.PAYMTERMID,
                                        @TAXWITHHOLDVENDORTYPE_TH = venderTable.VENDTYPEID,
                                        @BLOCKED = isactive,
                                        @VENDGROUP = vendGroup,
                                        @ACCOUNTNUM = venderTable.VENDCODE_AX,
                                        @DATAAREAID = venderTable.DATAAREAID
                                    },
                                    unitOfWork2.Transaction
                                );
                                unitOfWork2.Complete();
                            }
                        }

                        else
                        {
                            var vendcodeax = venderTable.VENDCODE_AX;
                            int isactive = 0;
                            var isactivesub = "";
                            if (venderTable.ISACTIVE == 0)
                            {
                                isactive = 2;
                                isactivesub = "Inactive";
                            }
                            else
                            {
                                isactive = 0;
                                isactivesub = "Active";
                            }
                            //isactive
                            venderTable.APPROVESTATUS = RequestStatusModel.Successfully;
                            venderTable.SENDMAILSUCCESS = 1;
                            venderTable.ISACTIVE = isactive;
                            venderTable.UPDATEDATE = DatetimeNow;
                            var emailcomplete = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                            /*foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1 && x.IsActive == 1))
                            {
                                emailcomplete.Add(emaillog.Email);
                            }*/
                            emailcomplete.Add(UserALL.Email);
                            var sendCompleteEmail = _emailService.SendEmail(
                                     //Subject : 2020/27/00009 / V-4765 /���Թ��� Active ���º��������
                                     $"Subject : {venderTable.REQUESTCODE} / {venderTable.VENDCODE_AX} / ���Թ��� {isactivesub} ���º��������",
                                    $@"
                                     <b>���Թ��� {isactivesub} ���º��������</b> <br/><br/>
                                     <b>�Ţ���Ӣ�   {venderTable.REQUESTCODE} </b> <br/>
                                     <b>���ʼ����    {venderTable.VENDCODE_AX} </b> <br/>
                                     <b>���ͼ����    {venderTable.VENDNAME} </b> <br/>  <br/><br/><br/>
                                 ",
                                    emailcomplete,
                                    new List<string> { }
                                );

                            if (sendCompleteEmail.Result == false)
                            {
                                throw new Exception(sendCompleteEmail.Message);
                            }
                            // 
                            //test
                            //update ax

                            var connString = "AXCust";
                            using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
                            {
                                var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE VENDTABLE
                                        SET BLOCKED = @a
                                    WHERE ACCOUNTNUM =  @b
                                    AND DATAAREAID = @c",
                                    new
                                    {
                                        @a = isactive,
                                        @b = vendcodeax,
                                        @c = venderTable.DATAAREAID
                                    },
                                    unitOfWork2.Transaction
                                );
                                unitOfWork2.Complete();
                            }
                        }


                    }
                    //is approve or reject?
                    else if (ApproveResult == 2 && ApproveRemark != null)
                    {
                        venderTable.APPROVESTATUS = RequestStatusModel.Reject;
                        //var fileok = new List<string>();
                        var filePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                        var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => x.REQUESTID == venderTable.ID && x.ISACTIVE == 1))
                        {
                            string fileName = filelog.FILENAME;
                            //fileok.Add($"{filePath}/{fileName}");
                        }

                        var emailreject = new List<string>();
                        var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                        foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenFailed == 1 && x.IsActive == 1 && x.ApproveLevel < venderTable.CURRENTAPPROVESTEP))
                        {
                            emailreject.Add(emaillog.Email);
                        }
                        emailreject.Add(UserALL.Email);
                        var vendercode = venderTable.VENDCODE_AX;
                        
                        var approveFlowName = ApproveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && x.ApproveLevel == venderTable.CURRENTAPPROVESTEP && x.IsActive == 1).Select(s => s.Name).FirstOrDefault();
                        var ApproveBy = approveFlowName;

                        var isactivesub = "";
                        if (venderTable.ISACTIVE == 0)
                        {
                            isactivesub = " ���ЧѺ����˹�����к� ";
                        }
                        else
                        {
                            isactivesub = " ��¡��ԡ����ЧѺ����˹��� ";
                        }
                        var sendRejectEmail = _emailService.SendEmail(
                                $"Subject : {venderTable.REQUESTCODE} / Reject / ��ͧ������{isactivesub} {vendercode}",
                                $@"
                                    <b> ��ͧ������{isactivesub} {vendercode} </b><br/>
                                    <b> �����ͧ��  </b> {EmpFullName}<br/><br/>
                                    <b> ���ʼ���� </b> {vendercode}<br/>
                                    <b> ���ͼ����  </b> {venderTable.VENDNAME}<br/>
                                    <b> �������     </b> {venderTable.ADDRESS}<br/>
                                    <b> ���ͼ��Դ��� </b> {venderTable.CONTACTNAME}<br/>
                                    <b> ���Ѿ��  </b> {venderTable.TEL}<br/>
                                    <b style='color:red'>���˵ط��{isactivesub}  </b> {venderTable.REMARK}<br/>
                                    <br/><br/>
                                    <b style='color:red'> ���˵ء����ͧ�� Reject : <b/> <b style='color:black'>{ApproveRemark}</b>
                                    <br/><br/>
                                    <b style='color:black'> Reject By </b> <b style='color:black'>{ApproveBy}</b><br/></br/>
                                 ",
                                emailreject,
                                new List<string> { },
                                "",
                                "",
                                new List<string> { }
                            );

                        if (sendRejectEmail.Result == false)
                        {
                            throw new Exception(sendRejectEmail.Message);
                        }
                    }
                    
                    else
                    {
                        // update head table
                        venderTable.CURRENTAPPROVESTEP += 1;
                        venderTable.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new VenderNonce_TB
                        {
                            NonceKey = nonceKey,
                            CreateDate = DatetimeNow,
                            ExpireDate = DatetimeNow.AddDays(7),
                            IsUsed = 0
                        });

                        //var fileok = new List<string>();
                        var filePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                        var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => (x.REQUESTID == venderTable.ID && x.ISACTIVE == 1) || (x.REQUESTID == venderTable.ID && x.ISTEMP == 1)))
                        {
                            string fileName = filelog.FILENAME;
                            //fileok.Add($"{filePath}/{fileName}");
                        }
                        var subject = "";
                        var body = "";
                        if (venderTable.ISACTIVE == 0)
                        {
                            subject += "/ Inactive / ��ͧ�������ЧѺ����˹�����к� / ";
                            body += "��ͧ�������ЧѺ����˹�����к�";
                        }
                        else
                        {
                            subject += "/ Active / ��ͧ������¡��ԡ����ЧѺ����˹��� / ";
                            body += "��ͧ������¡��ԡ����ЧѺ����˹���";
                        }

                        // next approve trans
                        //var nextApproveTrans = await unitOfWork.VenderControl.GetApproveTransByLevel(venderTable.ID, venderTable.CURRENTAPPROVESTEP, approvemasterid);
                        var nextALL = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.REQUESTID == venderTable.ID &&
                                                            x.APPROVELEVEL == venderTable.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1);
                        foreach (var i in nextLevel)
                        {
                            var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                            var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                            var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == venderTable.APPROVEMASTERID &&
                                                                    x.ApproveLevel == venderTable.CURRENTAPPROVESTEP &&
                                                                    x.IsActive == 1 && x.Email == i.EMAIL).Select(s => s.Name).FirstOrDefault();
                            var ApproveBy = approveFlowName;

                            var vendTbTempRepo3 = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                            var vendTbTempALL3 = await vendTbTempRepo3.GetAllAsync();
                            var vendTbTemp3 = vendTbTempALL3.Where(x => x.REQUESTID == VendID && x.ISCOMPLETE == 0).FirstOrDefault();
                            if (vendTbTemp3 != null)
                            {
                                var sendNextEmail = _emailService.SendEmail(
                                   $"Subject : {venderTable.REQUESTCODE} {subject} {venderTable.VENDCODE_AX} ",
                                    $@"
                                    <b>{body} {venderTable.VENDCODE_AX} </b> <br/>
                                    <b> �����ͧ��  </b> {EmpFullName}<br/><br/>
                                    <b> ���ʼ���� </b> {venderTable.VENDCODE_AX}<br/>
                                    <b> ���ͼ����  </b> {venderTable.VENDNAME}<br/>
                                    <b> �������     </b> {vendTbTemp3.ADDRESS_TEMP}<br/>
                                    <b> ���ͼ��Դ��� </b> {vendTbTemp3.CONTACTNAME_TEMP}<br/>
                                    <b> ���Ѿ��  </b> {vendTbTemp3.TEL_TEMP}<br/>
                                    <br/><br/><br/>
                                    <b style='color:red'>���˵ط��{body}  </b> {vendTbTemp3.REMARK_TEMP}<br/>
                                    <br/><br/><br/>
                                    <b> ���ͼ��ӡ��͹��ѵ� </b> {ApproveBy}<br/></br/>
                                    <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>��ԡ�����</a> <br/>
                                    
                                    ",
                                    //<b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_isActive?VendID={VendID}&TranID={nextApproveTrans.ID}&nonce={nonceKey}'>��ԡ�����</a> <br/>
                                    new List<string> { i.EMAIL },
                                    new List<string> { },
                                    "",
                                    "",
                                    new List<string> { }
                                );
                                if (sendNextEmail.Result == false)
                                {
                                    throw new Exception(sendNextEmail.Message);
                                }
                            }
                            else
                            {
                                var sendNextEmail = _emailService.SendEmail(
                                   $"Subject : {venderTable.REQUESTCODE} {subject} {venderTable.VENDCODE_AX} ",
                                    $@"
                                    <b>{body} {venderTable.VENDCODE_AX} </b> <br/>
                                    <b> �����ͧ��  </b> {EmpFullName}<br/><br/>
                                    <b> ���ʼ���� </b> {venderTable.VENDCODE_AX}<br/>
                                    <b> ���ͼ����  </b> {venderTable.VENDNAME}<br/>
                                    <b> �������     </b> {venderTable.ADDRESS}<br/>
                                    <b> ���ͼ��Դ��� </b> {venderTable.CONTACTNAME}<br/>
                                    <b> ���Ѿ��  </b> {venderTable.TEL}<br/>
                                    <br/><br/><br/>
                                    <b style='color:red'>���˵ط��{body}  </b> {venderTable.REMARK}<br/>
                                    <br/><br/><br/>
                                    <b> ���ͼ��ӡ��͹��ѵ� </b> {ApproveBy}<br/></br/>
                                    <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>��ԡ�����</a> <br/>
                                    ",
                                    // <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_isActive?VendID={VendID}&TranID={nextApproveTrans.ID}&nonce={nonceKey}'>��ԡ�����</a> <br/>
                                    new List<string> { i.EMAIL },
                                    new List<string> { },
                                    "",
                                    "",
                                    new List<string> { }
                                );
                                if (sendNextEmail.Result == false)
                                {
                                    throw new Exception(sendNextEmail.Message);
                                }
                            }
                            var approveTrans_next = await approveTransRepo.GetAsync(i.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await approveTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    int isapp, isreject;
                    if (ApproveResult == 1)
                    {
                        isapp = 1;
                        isreject = 0;
                    }
                    else
                    {
                        isapp = 0;
                        isreject = 1;
                    }
                    var newLogsApproveRepo = new GenericRepository<VenderLogsApprove_TB>(unitOfWork.Transaction);
                    var newLogsApprove = new VenderLogsApprove_TB
                    {
                        REQUESTID = VendID,
                        EMAIL = email,
                        CREATEDATE = DatetimeNow,
                        ISAPPROVE = isapp,
                        ISREJECT = isreject
                    };
                    var LogsApprove = await newLogsApproveRepo.InsertAsync(newLogsApprove);

                    await venderTableRepo.UpdateAsync(venderTable);
                    await nonceRepo.UpdateAsync(_nonce);



                    unitOfWork.Complete();


                    AlertSuccess = "���Թ����������";
                    return Redirect($"/Vender/VenderRequestList?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/");
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
                    isActive = vend.ISACTIVE;
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
                    isActive = vend.ISACTIVE;
                    ProductType = vend.PRODTYPEID;
                    Paymterm = vend.PAYMTERMID;
                    ProductTypeDetail = vend.PRODTYPEDETAIL;

                    var vendGroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                    var vendGroupALL = await vendGroupRepo.GetAllAsync();
                    var vendTypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                    var vendTypeALL = await vendTypeRepo.GetAllAsync();

                    VenderGroup = vendGroupALL.Where(x => x.ID == vend.VENDGROUPID).Select(s => s.DESCRIPTION).FirstOrDefault();
                    VenderType = vendTypeALL.Where(x => x.ID == vend.VENDTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();

                    var vendDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                    var vendDocALL = await vendDocRepo.GetAllAsync();
                    foreach (var docLog in vendDocALL.Where(x => x.REQUESTID == id && x.ISACTIVE == 1))
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

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnPostGridApproveAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LogApproveGridViewModel>($@"
                        SELECT *
                        FROM
                        (
                        SELECT VT.ID,VT.REQUESTID,
	                        VT.APPROVEMASTERID,VT.EMAIL,
	                        VT.APPROVELEVEL,
	                        CONVERT(VARCHAR,VT.APPROVEDATE,103)+' '+
	                        CONVERT(VARCHAR,VT.APPROVEDATE,108) APPROVEDATE,
	                        VG.DESCRIPTION,AF.Name
	                        --,ROW_NUMBER() OVER(PARTITION BY VT.REQUESTID,VT.APPROVEMASTERID,VT.APPROVELEVEL ORDER BY VT.ID DESC) AS Row
	                        FROM TB_VenderApproveTrans VT JOIN
	                        TB_VenderApproveMapping VM ON VT.APPROVEMASTERID = VM.APPROVEMASTERID JOIN
	                        TB_VenderApproveGroup VG ON VM.APPROVEGROUPID = VG.ID JOIN
	                        TB_ApproveFlow AF ON VT.APPROVEMASTERID = AF.ApproveMasterId AND 
	                        VT.EMAIL = AF.Email AND VT.APPROVELEVEL = AF.ApproveLevel
	                        WHERE  VT.REQUESTID = {id} AND VT.APPROVEDATE IS NOT NULL 
	                        AND VT.ISCURRENTAPPROVE =1 
	                        GROUP BY VT.ID,VT.REQUESTID,
	                        VT.APPROVEMASTERID,VT.EMAIL,
	                        VT.APPROVELEVEL,
	                        CONVERT(VARCHAR,VT.APPROVEDATE,103)+' '+
	                        CONVERT(VARCHAR,VT.APPROVEDATE,108) ,
	                        VG.DESCRIPTION,AF.Name
                        )T
                        ORDER BY T.APPROVEDATE
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

        public async Task<IActionResult> OnGetViewFileUploadAsync(int id, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    var vendLogFile = await vendLogFileRepo.GetAsync(Fileid);

                    var vendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vendTable = await vendTableRepo.GetAsync(id);

                    var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5);
                    var fileName = vendLogFile.FILENAME;


                    var basePath = $"{filePath}/{fileName}";
                    if (!System.IO.File.Exists(basePath))
                    {
                        throw new Exception("File not found.");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(basePath);

                    return File(fileBytes, "application/x-msdownload", fileName);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
