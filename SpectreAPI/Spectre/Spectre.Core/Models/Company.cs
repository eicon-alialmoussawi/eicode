using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Company
    {
        public Company()
        {
            CompanyPackages = new HashSet<CompanyPackage>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool? IsDeleted { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<CompanyPackage> CompanyPackages { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
