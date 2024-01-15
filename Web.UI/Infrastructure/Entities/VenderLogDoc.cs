using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderLogDoc")]
    public class VenderLogDoc_TB
    {
        public int ID { get; set; }

        public int REQUESTID { get; set; }

        public int DOCID { get; set; }


        [StringLength(50)]
        public string DOCDESCRIPTION { get; set; }

        [StringLength(50)]
        public string REMARK { get; set; }

        public int CREATEBY { get; set; }

        public DateTime? CREATEDATE { get; set; }

        public int ISACTIVE { get; set; }

        public int ISTEMP { get; set; }

    }
}
