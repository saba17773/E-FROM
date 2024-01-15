using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ENewRequest")]
    public class S2ENewRequest_TB
    {
        public int ID { get; set; }
        public string REQUESTCODE { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string DEALER { get; set; }
        public string PRODUCTIONSITE { get; set; }
        public string DEALERADDRESS { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public decimal PRICE { get; set; }
        public string CURRENCYCODE { get; set; }
        public string PERUNIT { get; set; }
        public int ISCOMPAIRE { get; set; }
        public decimal QTY { get; set; }
        public string UNIT { get; set; }
        public string PLANT { get; set; }
        public string REMARK { get; set; }
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
