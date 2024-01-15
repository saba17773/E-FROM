using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Services.Common.Dto
{
    public class ResponseResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
