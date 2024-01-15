using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ENewRequestCompaire")]
    public class S2ENewRequestCompaire_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public string VENDORIDREF { get; set; }
        public string SUPPLIERNAMEREF { get; set; }
        public string DEALERREF { get; set; }
        public string PRODUCTIONSITEREF { get; set; }
        public string DEALERADDRESSREF { get; set; }
        public string ITEMCODEREF { get; set; }
        public string ITEMNAMEREF { get; set; }
        public decimal PRICEREF { get; set; }
        public string CURRENCYCODEREF { get; set; }
        public string PERUNITREF { get; set; }
        public int ISCURRENTCOMPAIRE { get; set; }
    }
}
