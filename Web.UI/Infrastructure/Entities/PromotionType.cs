using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PromotionType")]
    public class PromotionTypeTable
    {
        public int Id { get; set; }
        public string PromotionType { get; set; }
        public string ByType { get; set; }
    }
}