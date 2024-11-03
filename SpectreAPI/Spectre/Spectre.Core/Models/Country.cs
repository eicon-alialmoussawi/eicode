using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Country
    {
        public Country()
        {
            Awards = new HashSet<Award>();
            CompanyPackageDetails = new HashSet<CompanyPackageDetail>();
            ContactUs = new HashSet<ContactU>();
            SocioEonomics = new HashSet<SocioEonomic>();
        }

        public int CountryId { get; set; }
        public string NameEn { get; set; }
        public string Iso3 { get; set; }
        public string NameAr { get; set; }
        public Nullable<int> RegionId { get; set; }

        public virtual ICollection<Award> Awards { get; set; }
        public virtual ICollection<CompanyPackageDetail> CompanyPackageDetails { get; set; }
        public virtual ICollection<ContactU> ContactUs { get; set; }
        public virtual ICollection<SocioEonomic> SocioEonomics { get; set; }
    }
}
