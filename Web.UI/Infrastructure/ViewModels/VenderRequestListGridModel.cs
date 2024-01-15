using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class VenderRequestListGridModel
    {
		
		public int ID { get; set; }
		public int REQUESTID { get; set; }
		public int APPROVEMASTERID { get; set; }
		public string EMAIL { get; set; }
		public int APPROVELEVEL { get; set; }
		public string SENDEMAILDATE { get; set; }
		public string APPROVEDATE { get; set; }
		public string REJECTDATE { get; set; }
		public int ISDONE { get; set; }
		public string REMARK { get; set; }
		public int ISCURRENTAPPROVE { get; set; }
		public string PROCESS { get; set; }
		public string REQUESTCODE { get; set; }
		public string REQUESTDATE { get; set; }
		public string VENDCODE { get; set; }
		public string VENDCODEAX { get; set; }
		public string VENDIDNUM { get; set; }
		public string VENDNAME { get; set; }
		public int CREATEBY { get; set; }
		public string EMPLOYEEID { get; set; }
		public string CREATENAME { get; set; }
		public string NONCEKEY { get; set; }
		public DateTime? EXPIREDATE { get; set; }
		public int ISUSED { get; set; }

	}
}
