using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_USER_APP")]
    public class EAAPP_UserApp_TABLE
    {

        public int USER_APP_CODE { get; set; }
        public string EMP_CODE { get; set; }
        public string HOST_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string PROJECT_NAME { get; set; }
        public DateTime? CREATE_DATE { get; set; }
    }
}
