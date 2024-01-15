using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Services.DataTable.Dto;

namespace Web.UI.Services.DataTable.Interface
{
    public interface IDtService
    {
        DtParameters GetParameters(HttpRequest request);
        DtResult<T> GetResult<T>(List<T> data, DtParameters dtParemeters);
    }
}
