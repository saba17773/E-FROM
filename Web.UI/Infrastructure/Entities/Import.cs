using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Import")]
    public class ImportTable
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
