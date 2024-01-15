using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class CustomerTypeTransViewModel
    {
        public int Id { get; set; }
        public int CCId { get; set; }
        public int CustomerByProductId { get; set; }
        public string ByName { get; set; }
        public string CustomerCode { get; set; }
    }
}