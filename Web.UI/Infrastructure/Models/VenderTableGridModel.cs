using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class VenderTableGridModel
    {
        public int ID { get; set; }
        public string REQUESTCODE { get; set; }
        public string REQUESTDATE { get; set; }
        public string VENDCODE { get; set; }
        public string VENDIDNUM { get; set; }
        public string VENDNAME { get; set; }
        public string VENDTYPECODE { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACTNAME { get; set; }
        public string TEL { get; set; }
        public string FAX { get; set; }
        public string WEBSITE { get; set; }
        public string EMAIL { get; set; }
        public int VENDGROUPID { get; set; }
        public string CURRENCY { get; set; }
        public string PRODTYPEID { get; set; }
        public int PROJECTID { get; set; }

        public string REMARK { get; set; }
        public int APPROVESTATUS { get; set; }
        public int COMPLETEBY { get; set; }

        public string COMPLETEDATE { get; set; }
        public int CREATEBY { get; set; }

        public DateTime CREATEDATE { get; set; }

        public int UPDATEBY { get; set; }

        public DateTime UPDATEDATE { get; set; }

        public int TotalApproveStep { get; set; }

        public int CURRENTAPPROVESTEP { get; set; }

        public int VENDPROCESSID { get; set; }

        public string VENDPROCESSDESC { get; set; }
    
        public int SENDMAILSUCCESS { get; set; }

        public int ISACTIVE { get; set; }

        public string vendcode_ax { get; set; }

        public int ISREVISE { get; set; }
        public string USERNAME { get; set; }
        public string DATAAREAID { get; set; }
    }
}
