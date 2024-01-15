using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class MethodOfPaymentModel
    {
        public static string NOT_PAY { get; set; } = "รับผิดชอบการส่งของถึงมือลูกค้า";
        public static string COMPANY_PAY { get; set; } = "ส่งของที่บริษัทขนส่ง/รับผิดชอบค่าขนส่ง";
        public static string CLIENT_PAY { get; set; } = "ส่งของที่บริษัทขนส่ง/ลูกค้าจ่ายค่าขนส่ง";
        public static string OTHER { get; set; } = "อื่นๆ โปรดระบุ";

        public static string GetText(string permissionKey)
        {
            foreach (var item in typeof(MethodOfPaymentModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(MethodOfPaymentModel)).ToString();
            }

            return "";
        }
    }
}
