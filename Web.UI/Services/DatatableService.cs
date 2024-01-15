using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.UI.Interfaces;

namespace Web.UI.Services
{
  public class DatatableService : IDatatableService
  {
    private IHelperService _helperService;

    public DatatableService(IHelperService helperService)
    {
      _helperService = helperService;
    }

    public string Filter(HttpRequest req, object field)
    {
      try
      {
        var columns = new List<string>();
        var search = req.Form["search[value]"].FirstOrDefault();
        var sortColumnId = req.Form["order[0][column]"].FirstOrDefault();
        var sortColumnName = req.Form["columns[" + sortColumnId + "][data]"].FirstOrDefault();
        var sortColumnDirection = req.Form["order[0][dir]"].FirstOrDefault();

        if (search == null)
        {
          return "";
        }

        Regex regex = new Regex(@"columns\[(\d+)\]\[data\]");

        foreach (var item in req.Form)
        {
          Match match = regex.Match(item.Key);
          if (match.Success)
          {
            string fieldName = req.Form[match.Value.ToString()];
            if (fieldName != null && fieldName != "")
            {
              // var a = nameof(field.GetField(""));
              columns.Add(GetField(fieldName, field));
            }
          }
        }

        string query = "";
        string order = "";

        if (columns.Count > 0)
        {
          for (int i = 0; i < columns.Count; i++)
          {
            if (i != columns.Count - 1)
            {
              query += $" {columns[i]} LIKE '%{search}%' OR ";
            }
            else
            {
              query += $" {columns[i]} LIKE '%{search}%' ";
            }
          }
        }

        //if (sortColumnName != null && sortColumnDirection != null && int.Parse(sortColumnId) != 0)
        //{
        //    order = $" ORDER BY {sortColumnName} {sortColumnDirection} ";
        //}

        if (query == "")
        {
          query = " 1=1 ";
        }

        return " ( " + query + " ) " + order;
      }
      catch (System.Exception)
      {
        throw;
      }
    }

    public object Format<T>(HttpRequest req, List<T> data)
    {
      try
      {
        var draw = req.Form["draw"].FirstOrDefault();
        var start = req.Form["start"].FirstOrDefault();
        var length = req.Form["length"].FirstOrDefault();
        var sortType = req.Form["order[0][dir]"].FirstOrDefault();
        var sortColumnId = req.Form["order[0][column]"].FirstOrDefault();
        var sortColumnName = req.Form["columns[" + sortColumnId + "][data]"].FirstOrDefault();
        var sortColumnDirection = req.Form["order[0][dir]"].FirstOrDefault();
        var searchValue = req.Form["search[value]"].FirstOrDefault();
        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        int skip = start != null ? Convert.ToInt32(start) : 0;

        return new
        {
          draw = draw,
          recordsFiltered = data.Count,
          recordsTotal = data.Count,
          data = data.Skip(skip).Take(pageSize),
        };
      }
      catch (System.Exception)
      {
        throw;
      }
    }

    public object FormatOnce<T>(List<T> data)
    {
      try
      {
        return new
        {
          draw = 1,
          recordsFiltered = data.Count,
          recordsTotal = data.Count,
          data = data
        };
      }
      catch (System.Exception)
      {
        throw;
      }
    }

    public string GetField(string column, object field)
    {
      try
      {
        var val = _helperService.GetPropertyValue(field, column);

        if (val != null)
        {
          return val.ToString();
        }

        return column;
      }
      catch (System.Exception)
      {
        throw;
      }
    }
  }
}
