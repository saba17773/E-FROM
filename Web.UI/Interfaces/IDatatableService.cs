using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Interfaces
{
    public interface IDatatableService
    {
        object Format<T>(HttpRequest req, List<T> data);
        string Filter(HttpRequest req, object field);
        object FormatOnce<T>(List<T> data);
        string GetField(string column, object field);
    }
}
