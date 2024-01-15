using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_LOG_APP")]
    public class EAAPP_LogApp_TABLE
    {
        public int LOG_CODE { get; set; }
        public string EMP_CODE { get; set; }
        public string USER_NAME { get; set; }
        public string HOST_NAME { get; set; }
        public DateTime? LOGIN_DATE { get; set; }
        public DateTime? LOGOUT_DATE { get; set; }
        public string PROJECT_NAME { get; set; }
    }
}
