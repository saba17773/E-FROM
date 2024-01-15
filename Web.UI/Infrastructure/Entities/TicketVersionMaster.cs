using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_TicketVersionMaster")]
    public class TicketVersionMasterTable
    {
        public int Id { get; set; }
        public string Version { get; set; }
    }
}