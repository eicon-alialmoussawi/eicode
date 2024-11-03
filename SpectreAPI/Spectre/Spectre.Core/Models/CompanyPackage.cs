using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class CompanyPackage
    {
        public CompanyPackage()
        {
            CompanyPackageDetails = new HashSet<CompanyPackageDetail>();
        }

        public int Id { get; set; }
        public int? PackageId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Price { get; set; }
        public int? Currency { get; set; }
        public int? NumberOfUsers { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Company Company { get; set; }
        public virtual Lookup CurrencyNavigation { get; set; }
        public virtual Package Package { get; set; }
        public virtual ICollection<CompanyPackageDetail> CompanyPackageDetails { get; set; }
    }
}
