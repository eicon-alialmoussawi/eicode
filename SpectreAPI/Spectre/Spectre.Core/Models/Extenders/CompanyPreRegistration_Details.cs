using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class CompanyPreRegistration_Details
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? PreferredPackage { get; set; }
        public string Message { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsViewed { get; set; }
        public DateTime? CreationDate { get; set; }
        public string PackageName { get; set; }
        public string CompanyName { get; set; }
    }
}
