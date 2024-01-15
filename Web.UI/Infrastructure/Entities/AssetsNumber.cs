using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("ASSETTABLE")]
    public class AssetsNumberTable
    {
        public string NameAlias { get; set; }
        public string DataareaId { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
    }
}
