using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EAddMoreRawMaterialLogsSendEmail")]
    public class S2EAddMoreRawMaterialLogsSendEmail_TB
    {
        public int ID { get; set; }
        public int RMREQID { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public string EMAIL { get; set; }
        public int SENDEMAILBY { get; set; }
        public DateTime? SENDEMAILDATE { get; set; }
        public int ISLASTSENDEMAIL { get; set; }
    }
}
