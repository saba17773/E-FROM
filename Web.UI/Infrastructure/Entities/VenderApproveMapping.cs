using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderApproveMapping")]
    public class VenderApproveMapping_TB
    {
        public int ID { get; set; }

        public int CreateBy { get; set; }

        public string DESCRIPTION { get; set; }

        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int STEP { get; set; }
        public string DATAAREAID { get; set; }
    }
}
