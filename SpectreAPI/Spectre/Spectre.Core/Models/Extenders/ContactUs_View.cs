using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class ContactUs_View
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Message { get; set; }
        public int? CountryId { get; set; }
        public bool? IsDeleted { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }
}
