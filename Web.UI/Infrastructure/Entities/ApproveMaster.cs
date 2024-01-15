using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ApproveMaster")]
    public class ApproveMasterTable
    {
        public int Id { get; set; }

        [Required]
        public string GroupDescription { get; set; }

        [Required]
        public int IsActive { get; set; }
        public int isS2EProject { get; set; }

    }
}
