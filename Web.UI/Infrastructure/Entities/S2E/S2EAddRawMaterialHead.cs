using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EAddRawMaterialHead")]
    public class S2EAddRawMaterialHead_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public int PCSAMPLEID { get; set; }
        public string PLANT { get; set; }
        public int ISACTIVE { get; set; }
        public int ISADDMORE { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string CURRENCYCODE { get; set; }
    }
}
