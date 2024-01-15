using System;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EReportStockCardGridModel
	{
		public int NEWREQUESTID { get; set; }
        public int ADDHEADID { get; set; }
		public int ADDLINEID { get; set; }
		public int REQHEADID { get; set; }
		public int REQLINEID { get; set; }
		public string REQUESTCODE { get; set; }
		public string PLANT { get; set; }
		public string POSTINGDATE { get; set; }
		public int CREATEBY { get; set; }
		public string REQUESTBY { get; set; }
		public string MOVEMENT { get; set; }
		public string SAMPLETYPE { get; set; }
		public string ITEMCODE { get; set; }
		public string ITEMNAME { get; set; }
		public decimal QTY { get; set; }
		public string UNIT { get; set; }
		public string DOCNO { get; set; }
		public decimal SORTCREATEDATE { get; set; }
		public int COUNTMAX { get; set; }
		public int ROWNEWREQUEST { get; set; }
	}
}
