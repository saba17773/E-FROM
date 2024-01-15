using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PromotionTypeFrom")]
    public class PromotionTypeFromTable
    {
        public int Id { get; set; }
        public string PromotionFrom { get; set; }
    }
}