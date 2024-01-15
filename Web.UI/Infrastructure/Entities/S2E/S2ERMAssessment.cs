using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ERMAssessment")]
    public class S2ERMAssessment_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string PLANT { get; set; }
        public int DEPARTMENTID { get; set; }
        public string DEPARTMENTREMARK { get; set; }
        public string MATERIALGROUP { get; set; }
        public string MATERIALNAME { get; set; }
        public string ITEMGROUP { get; set; }
        public decimal SUPPLIERD1 { get; set; }
        public decimal SUPPLIERD2 { get; set; }
        public decimal SUPPLIERD3 { get; set; }
        public decimal SUPPLIERD4 { get; set; }
        public decimal SUPPLIERD5 { get; set; }
        public decimal SUPPLIERREFD1 { get; set; }
        public decimal SUPPLIERREFD2 { get; set; }
        public decimal SUPPLIERREFD3 { get; set; }
        public decimal SUPPLIERREFD4 { get; set; }
        public decimal SUPPLIERREFD5 { get; set; }
        public decimal MONTHSAVECOST { get; set; }
        public decimal YEARSAVECOST { get; set; }
        public string PLANTCODED1D2 { get; set; }
        public string PLANTCODED3 { get; set; }
        public string PLANTCODED4 { get; set; }
        public string PLANTCODED5 { get; set; }
        public int ISSTARTTEST { get; set; }
        public string ISSTARTTESTREMARK { get; set; }
        public int ISECOTOXIC { get; set; }
        public string ISECOTOXICREMARK { get; set; }
        public int ISHUMANTOXIC { get; set; }
        public string ISHUMANTOXICREMARK { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int CURRENTAPPROVESTEP { get; set; }
        public int APPROVESTATUS { get; set; }
        public string CANCELREMARK { get; set; }
        public int ISACTIVE { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public int COMPLETEBY { get; set; }
        public DateTime? COMPLETEDATE { get; set; }
    }
}
