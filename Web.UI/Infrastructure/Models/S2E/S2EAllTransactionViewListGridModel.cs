using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EAllTransactionViewListGridModel
    {
        public int GROUPID { get; set; }
        public string GROUPDESCRIPTION { get; set; }
        public int ID { get; set; }
        public int LINEID { get; set; }
        public string STATUSAPPROVE { get; set; }
        public string COLOUR { get; set; }
        public string URLVIEWDETAIL { get; set; }
        public string PAGENAME { get; set; }
        public int ISHAVELINEID { get; set; }
        public int ORDERBYPROCESS { get; set; }
        public int ISCANEDIT { get; set; }
        public int ISEDIT { get; set; }
    }
}
