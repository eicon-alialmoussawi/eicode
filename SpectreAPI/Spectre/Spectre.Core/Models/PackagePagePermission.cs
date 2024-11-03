using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class PackagePagePermission
    {
        public PackagePagePermission()
        {
            CompanyPackageDetails = new HashSet<CompanyPackageDetail>();
        }

        public int Id { get; set; }
        public int? PackageId { get; set; }
        public string PageUrl { get; set; }
        public bool? HasCountryLimit { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Package Package { get; set; }
        public virtual ICollection<CompanyPackageDetail> CompanyPackageDetails { get; set; }
    }
}
