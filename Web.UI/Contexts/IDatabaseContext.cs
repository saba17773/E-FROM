using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Contexts
{
    public interface IDatabaseContext
    {
        IDbConnection GetConnection(string connString = null);
    }
}
