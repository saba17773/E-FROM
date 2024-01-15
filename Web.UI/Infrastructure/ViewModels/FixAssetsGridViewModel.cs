using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class FixAssetsGridViewModel
    {
        public int Id { get; set; }
        public string AssetNumber { get; set; }
        public string AssetDate { get; set; }
        public string AssetCategory { get; set; }
        public string AssetType { get; set; }
        public string Company { get; set; }
        public int CurrentApproveStep { get; set; }
        public int TotalApproveStep { get; set; }
        public string AssetCondition { get; set; }
        public string AssetCause { get; set; }
        public int CreateBy { get; set; }
        public string CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public string UpdateDate { get; set; }
        public string RequestStatus { get; set; }
        public string FullName { get; set; }
        public int IdFile { get; set; }
        public string KeyNumber { get; set; }
        public string DepKeyNumber { get; set; }
        public int UpdateAx { get; set; }


    }
}