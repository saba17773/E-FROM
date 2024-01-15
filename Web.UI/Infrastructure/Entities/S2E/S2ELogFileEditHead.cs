using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELogFileEditHead")]
    public class S2ELogFileEditHead_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int CURRENTAPPROVESTEP { get; set; }
        public int APPROVESTATUS { get; set; }
        public string REMARK { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public int COMPLETEBY { get; set; }
        public DateTime? COMPLETEDATE { get; set; }
    }
}
