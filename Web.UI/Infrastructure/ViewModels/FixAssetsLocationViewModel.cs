using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class FixAssetsLocationViewModel
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string Location { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public string EquipmentUs { get; set; }
        public int RoomCode { get; set; }

    }
}