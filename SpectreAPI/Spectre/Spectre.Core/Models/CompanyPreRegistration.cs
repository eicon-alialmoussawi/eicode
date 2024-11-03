using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class CompanyPreRegistration
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
        public string CompanyName { get; set; }

        public virtual Package PreferredPackageNavigation { get; set; }
    }
}
