using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Package
    {
        public Package()
        {
            CompanyPackages = new HashSet<CompanyPackage>();
            CompanyPreRegistrations = new HashSet<CompanyPreRegistration>();
            PackagePagePermissions = new HashSet<PackagePagePermission>();
        }

        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameSpa { get; set; }
        public string NameFr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionSpa { get; set; }
        public string DescriptionFr { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsDemoPackage { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsDeleted { get; set; }
        public int? FromYearLimit { get; set; }
        public int? ToYearLimit { get; set; }
        public int? Order { get; set; }

        public virtual ICollection<CompanyPackage> CompanyPackages { get; set; }
        public virtual ICollection<CompanyPreRegistration> CompanyPreRegistrations { get; set; }
        public virtual ICollection<PackagePagePermission> PackagePagePermissions { get; set; }
    }
}
