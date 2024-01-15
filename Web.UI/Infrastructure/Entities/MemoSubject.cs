using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_MemoSubject")]
    public class MemoSubjectTable
    {
        public int Id { get; set; }
        public string MemoSubject { get; set; }
    }
}
