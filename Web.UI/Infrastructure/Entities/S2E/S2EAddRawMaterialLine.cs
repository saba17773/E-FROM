using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EAddRawMaterialLine")]
    public class S2EAddRawMaterialLine_TB
    {
        public int ID { get; set; }
        public int ADDRMID { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string PONO { get; set; }
        public decimal QTY { get; set; }
        public decimal QTYPO { get; set; }
        public decimal PRICE { get; set; }
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
        public int ISCURRENTLOGS { get; set; }
        public int ISPURCHASESAMPLE { get; set; }
    }
}
