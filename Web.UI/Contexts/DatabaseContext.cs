using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Contexts
{
  public class DatabaseContext : IDatabaseContext
  {
    private IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IDbConnection GetConnection(string connString = null)
    {
      if (connString == null)
      {
        return new SqlConnection(_configuration.GetConnectionString("Default"));
      }
      else
      {
        return new SqlConnection(_configuration.GetConnectionString(connString));
      }
    }
  }
}
