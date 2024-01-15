using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Services.DataTable.Dto;
using Web.UI.Services.DataTable.Interface;

namespace Web.UI.Services.DataTable.Handler
{
    public class DtService : IDtService
    {
        public DtParameters GetParameters(HttpRequest request)
        {
            var orderColumn = "";
            var reqSearch = "";
            var reqDraw = "";
            var reqStart = "";
            var reqLength = "";
            var orderColumnNumber = "";
            var orderDir = "";

            if (request.Method == "GET")
            {
                reqSearch = request.Query["search[value]"].FirstOrDefault() ?? "";
                reqDraw = request.Query["draw"].FirstOrDefault() ?? "1";
                reqStart = request.Query["start"].FirstOrDefault();
                reqLength = request.Query["length"].FirstOrDefault();

                orderColumnNumber = request.Query["order[0][column]"].FirstOrDefault() ?? "";
                orderDir = request.Query["order[0][dir]"].FirstOrDefault() ?? "";
                orderColumn = request.Query[$"columns[{orderColumnNumber}][data]"].FirstOrDefault() ?? "";
            }
            else
            {
                reqSearch = request.Form["search[value]"].FirstOrDefault() ?? "";
                reqDraw = request.Form["draw"].FirstOrDefault() ?? "1";
                reqStart = request.Form["start"].FirstOrDefault();
                reqLength = request.Form["length"].FirstOrDefault();

                orderColumnNumber = request.Form["order[0][column]"].FirstOrDefault() ?? "";
                orderDir = request.Form["order[0][dir]"].FirstOrDefault() ?? "";
                orderColumn = request.Form[$"columns[{orderColumnNumber}][data]"].FirstOrDefault() ?? "";
            }


            string column = "";
            var nestCol = orderColumn.Split(".");

            for (int i = 0; i < nestCol.Length; i++)
            {
                if (nestCol[i] != "")
                {
                    if (i == nestCol.Length - 1)
                    {
                        column += nestCol[i].Substring(0, 1).ToUpper() + nestCol[i].Substring(1);
                    }
                    else
                    {
                        column += nestCol[i].Substring(0, 1).ToUpper() + nestCol[i].Substring(1) + ".";
                    }
                }
            }

            int draw = Convert.ToInt32(reqDraw);
            int pageSize = reqLength != null ? Convert.ToInt32(reqLength) : 100;
            int skip = reqStart != null ? Convert.ToInt32(reqStart) : 0;

            return new DtParameters
            {
                Draw = draw,
                Search = reqSearch,
                Start = skip,
                Length = pageSize,
                OrderColumn = column,
                OrderDir = orderDir
            };
        }

        public DtResult<T> GetResult<T>(List<T> data, DtParameters dtParemeters)
        {
            return new DtResult<T>
            {
                Draw = dtParemeters.Draw,
                RecordsFiltered = data.Count,
                RecordsTotal = data.Count,
                Data = data.Skip(dtParemeters.Start).Take(dtParemeters.Length)
            };
        }
    }
}
