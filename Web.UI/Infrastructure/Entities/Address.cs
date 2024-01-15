using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Address")]
    public class AddressTable
    {
        public int Id { get; set; }

        [Required]
        public int CCId { get; set; }
        public string AddressType { get; set; }
        public string AddressNo { get; set; }
        public int? Moo { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public int? District { get; set; }
        public int? SubDistrict { get; set; }
        public int? Province { get; set; }
        public int? ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string ContactPerson1 { get; set; }
        public string ContactPerson1_Tel { get; set; }
        public string ContactPerson1_Email { get; set; }
        public string ContactPerson2 { get; set; }
        public string ContactPerson2_Tel { get; set; }
        public string ContactPerson2_Email { get; set; }
    }
}