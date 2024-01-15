using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_Company")]
    public class QingMaster_Company_TB
    {
        public int Id { get; set; }
        public string company { get; set; }
        public string FullName_TH { get; set; }
        public string FullName_EN { get; set; }
        public int IsQingCompany { get; set; }
        public string DataAreaId { get; set; }
        public int IsWMS { get; set; }
        public string Branchcode { get; set; }
    }
}
