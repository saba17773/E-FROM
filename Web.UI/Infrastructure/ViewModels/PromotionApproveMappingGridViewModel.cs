using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class PromotionApproveMappingGridViewModel
    {
        public int Id { get; set; }
        public string CCType { get; set; }
        public int ApproveMasterId { get; set; }
        public string GroupDescription { get; set; }
        public int CreateBy { get; set; }
        public string Username { get; set; }
        public string TypeProduct { get; set; }
    }
}