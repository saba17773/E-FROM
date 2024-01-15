using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_NumberSeq")]
    public class NumberSeqTable
    {
        public int Id { get; set; }
        public string SeqKey { get; set; }
        public int SeqValue { get; set; }
        public int SeqYear { get; set; }
    }
}
