using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PromotionByProduct")]
    public class PromotionByProductTable
    {
        public int Id { get; set; }
        public string ByCode { get; set; }
        public string ByName { get; set; }
        public string ByType { get; set; }
        public int SeqValue { get; set; }
        public int SeqYear { get; set; }
    }
}