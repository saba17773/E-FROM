using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ENewRequestNonce")]
    public class S2ENewRequestNonce_TB
    {
        public int ID { get; set; }
        public string NONCEKEY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public DateTime? EXPIREDATE { get; set; }
        public int ISUSED { get; set; }
    }
}
