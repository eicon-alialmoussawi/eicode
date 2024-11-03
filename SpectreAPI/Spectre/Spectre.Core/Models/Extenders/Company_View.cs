using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Company_View
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool? IsDeleted { get; set; }
        public string Logo { get; set; }

    }
}
