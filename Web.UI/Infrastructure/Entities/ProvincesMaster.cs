using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Provinces")]
    public class ProvincesMaster_TB
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string NameInThai { get; set; }
        public string NameInEnglish { get; set; }


        //
    }
}
