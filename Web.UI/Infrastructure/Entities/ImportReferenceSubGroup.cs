using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Dev_RefSubgroup_DEV")]
    public class ImportReferenceSubGroupTable
    {
        [ExplicitKey]
        public string SubgroupId { get; set; }
        public string RefSubgroupId { get; set; }
        public string GroupId { get; set; }
        public int isCheck { get; set; }
    }
}
