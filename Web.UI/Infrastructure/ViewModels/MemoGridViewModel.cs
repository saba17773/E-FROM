using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class MemoGridViewModel
    {
        public int Id { get; set; }
        public string MemoNumber { get; set; }
        public string MemoDate { get; set; }
        public string MemoAttn { get; set; }
        public string MemoSubject { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}