using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("DSG_WORKORDERTABLE")]
    public class AX_WORKORDERTABLE
    {
        public string DRIVER_ID { get; set; }
        public int DSG_TIME { get; set; }
        public decimal DSG_GROSSWEIGHT_IN { get; set; }
        public string DSG_ContainerNo { get; set; }
        public string DSG_SealNo { get; set; }
        public string Remarks { get; set; }
        public string VOUCHER_TYPE { get; set; }
        public string VOUCHER_SERIES { get; set; }
        public string VOUCHER_NO { get; set; }
        public string DSG_LOADINGPLANT { get; set; }
        public decimal DSG_GrossWeight_1 { get; set; }
        public decimal DSG_GrossWeight_2 { get; set; }

    }
}
