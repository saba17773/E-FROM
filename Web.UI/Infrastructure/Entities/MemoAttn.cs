using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_MemoAttn")]
    public class MemoAttnTable
    {
        public int Id { get; set; }
        public string MemoAttn { get; set; }
    }
}
