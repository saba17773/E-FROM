using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderTable_TEMP")]
    public class VenderTable_TempTB
    {
        public int ID { get; set; }

        public int REQUESTID { get; set; }

        public string ADDRESS_TEMP { get; set; }

        public string CONTACTNAME_TEMP { get; set; }

        public string TEL_TEMP { get; set; }

        public string FAX_TEMP { get; set; }

        public string WEBSITE_TEMP { get; set; }

        public string EMAIL_TEMP { get; set; }

        public int VENDGROUPID_TEMP { get; set; }

        public int VENDTYPEID_TEMP { get; set; }

        public string CURRENCY_TEMP { get; set; }

        public string PRODTYPEID_TEMP { get; set; }

        public string PAYMTERMID_TEMP { get; set; }

        public string REMARK_TEMP { get; set; }

        public int ISCOMPLETE { get; set; }

        public string PRODTYPEDETAIL_TEMP { get; set; }

    }
}
