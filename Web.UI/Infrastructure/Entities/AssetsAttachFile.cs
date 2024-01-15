using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsAttachFile")]
    public class AssetsAttachFileTable
    {
        public int Id { get; set; }
        public int FileNo { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int CCId { get; set; }
    }
}
