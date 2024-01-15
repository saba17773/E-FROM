using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELogFileEditNonce")]
    public class S2ELogFileEditNonce_TB
    {
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string NONCEKEY { get; set; }
        [Required]
        public DateTime? CREATEDATE { get; set; }
        [Required]
        public DateTime? EXPIREDATE { get; set; }
        [Required]
        public int ISUSED { get; set; }
    }
}
