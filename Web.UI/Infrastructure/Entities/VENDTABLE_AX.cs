using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("VENDTABLE")]
    public class VENDTABLE_AXTB
    {
        /*[Key]*/
        [Dapper.Contrib.Extensions.Key]
        public string ACCOUNTNUM { get; set; }

        public int BLOCKED { get; set; }
        
    }
}
