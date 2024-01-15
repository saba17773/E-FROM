using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public static class RequestTypeModel
    {
        public static string DOM { get => "DOM"; }
        public static string OVS { get => "OVS"; }

        public static string GetValue(string key)
        {
            foreach (var item in typeof(RequestTypeModel).GetProperties())
            {
                if (item.Name == key)
                    return item.GetValue(typeof(RequestTypeModel)).ToString();
            }

            return "";
        }
    }

}
