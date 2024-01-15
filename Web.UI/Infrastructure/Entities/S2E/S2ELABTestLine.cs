using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELABTestLine")]
    public class S2ELABTestLine_TB
    {
        public int ID { get; set; }
        public int LABID { get; set; }
        public string PROJECTREFNO { get; set; }
        public int DEPARTMENTID { get; set; }
        public string DEPARTMENTREMARK { get; set; }
        public int RESONTESTID { get; set; }
        public string RESONTESTREMARK { get; set; }
        public int TYPEOFRMID { get; set; }
        public string CHEMICALNAME { get; set; }
        public string CHEMICALNAMEREF { get; set; }
        public string ITEMCODE { get; set; }
        public string TRADENAME { get; set; }
        public string COUNTRY { get; set; }
        public string MANUFACTURE { get; set; }
        public string AGENT { get; set; }
        public string PLANTCODED1D2 { get; set; }
        public string PLANTCODED3 { get; set; }
        public string PLANTCODED4 { get; set; }
        public string PLANTCODED5 { get; set; }
        public string PLANTCODED1D2REF { get; set; }
        public string PLANTCODED3REF { get; set; }
        public string PLANTCODED4REF { get; set; }
        public string PLANTCODED5REF { get; set; }
        public int TESTRESULT { get; set; }
        public int APPROVALFORID { get; set; }
        public string PLANT { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int CURRENTAPPROVESTEP { get; set; }
        public int APPROVESTATUS { get; set; }
        public int ISPURCHASESAMPLE { get; set; }
        public decimal QTY { get; set; }
        public int ISACTIVE { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public int COMPLETEBY { get; set; }
        public DateTime? COMPLETEDATE { get; set; }
        public int ISCURRENTLOGS { get; set; }
        public string PurchaseRemark { get; set; }
    }
}
