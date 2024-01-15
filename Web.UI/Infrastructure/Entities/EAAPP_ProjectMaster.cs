using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PROJECT_MASTER")]
    public class EAAPP_ProjectMaster_TABLE
    {
        public int PROJECT_ID { get; set; }
        public string PROJECT_NAME { get; set; }
    }
}
