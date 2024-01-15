using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderTable")]
    public class VenderTable_TB
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string REQUESTCODE { get; set; }

        public DateTime? REQUESTDATE { get; set; }

        [StringLength(20)]
        public string VENDCODE { get; set; }

        [StringLength(20)]
        public string VENDIDNUM { get; set; }

        [StringLength(150)]
        public string VENDNAME { get; set; }

        /*[StringLength(3)]
        public string VENDTYPECODE { get; set; }*/

        [StringLength(250)]
        public string ADDRESS { get; set; }

        [StringLength(50)]
        public string CONTACTNAME { get; set; }

        [StringLength(20)]
        public string TEL { get; set; }

        [StringLength(20)]
        public string FAX { get; set; }

        [StringLength(100)]
        public string WEBSITE { get; set; }

        [EmailAddress]
        [StringLength(50)]
        public string EMAIL { get; set; }

        public int VENDGROUPID { get; set; }

        public int VENDTYPEID { get; set; }

        [StringLength(3)]
        public string CURRENCY { get; set; }

        public string PRODTYPEID { get; set; }

        public string PAYMTERMID { get; set; }

        /* public int PROJECTID { get; set; }*/

        [StringLength(250)]
        public string REMARK { get; set; }
        public int VENDPROCESSID { get; set; }
        public int APPROVESTATUS { get; set; }

        public int CURRENTAPPROVESTEP { get; set; }

        public int COMPLETEBY { get; set; }

        public DateTime? COMPLETEDATE { get; set; }

        public int CREATEBY { get; set; }

        public DateTime? CREATEDATE { get; set; }

        public int UPDATEBY { get; set; }

        public DateTime? UPDATEDATE { get; set; }

        public int APPROVEMASTERID { get; set; }
        public string VENDCODE_AX { get; set; }
        public int ISACTIVE { get; set; }

        public int SENDMAILSUCCESS { get; set; }

        public int ISREVISE { get; set; }
        public string PRODTYPEDETAIL { get; set; }
        public string DATAAREAID { get; set; }

    }
}
