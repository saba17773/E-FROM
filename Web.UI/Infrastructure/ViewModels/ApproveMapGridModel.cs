using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class ApproveMapGridModel
    {
        public int ID { get; set; }
        public int CreateBy { get; set; }
        public string DESCRIPTION { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int STEP { get; set; }
        public string EMPLOYEEID { get; set; }
        public string EMAIL { get; set; }
        public string GroupDescription { get; set; }
        public string DataAreaID { get; set; }
        public string Username { get; set; }
    }
}
