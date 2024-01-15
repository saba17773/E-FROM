using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaterialRequestLine")]
    public class S2EMaterialRequestLine_TB
    {
        public int ID { get; set; }
        public int ADDRMLINEID { get; set; }
        public int RMREQID { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string NO { get; set; }
        public decimal QTY { get; set; }
        public string DEPARTMENT { get; set; }
        public string SUPGROUP { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int CURRENTAPPROVESTEP { get; set; }
        public int APPROVESTATUS { get; set; }
        public int ISACTIVE { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public int COMPLETEBY { get; set; }
        public DateTime? COMPLETEDATE { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
