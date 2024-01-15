using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EPurchaseSample")]
    public class S2EPurchaseSample_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string PLANT { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int APPROVESTATUS { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int COMPLETEBY { get; set; }
        public DateTime? COMPLETEDATE { get; set; }

    }
}
