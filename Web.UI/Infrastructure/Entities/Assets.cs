using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssets")]
    public class AssetsTable
    {
        public int Id { get; set; }
        public string AssetNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? AssetDate { get; set; }
        [Required]
        public int AssetCategory { get; set; }
        
        public int AssetType { get; set; }
        [Required]
        public string Company { get; set; }
        public int CurrentApproveStep { get; set; }
        public int RequestStatus { get; set; }
        public string AssetCondition { get; set; }
        public string AssetCause { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int MoveFrom { get; set; }
        public int MoveTo { get; set; }
        public string ReceiveEmployee { get; set; }
        public string Phone { get; set; }
        public string KeyNumber { get; set; }
        public string EmployeeId { get; set; }
        public int UpdateAx { get; set; }
    }
}
