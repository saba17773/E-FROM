using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderLogFile")]
    public class VenderLogFile_TB
    {
        public int ID { get; set; }

        public int REQUESTID { get; set; }

        [StringLength(20)]
        public string FILENAME { get; set; }
        public int CREATEBY { get; set; }

        public DateTime? UPLOADDATE { get; set; }

        public int ISACTIVE { get; set; }

        public int ISTEMP { get; set; }
    }
}
