using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Currency")]
    public class CurrencyTable
    {
        public int Id { get; set; }
        public string CURRENCYCODE { get; set; }
        public string TXT { get; set; }
    }
}