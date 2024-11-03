using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Lookup
    {
        public Lookup()
        {
            AwardOperatorNavigations = new HashSet<Award>();
            AwardSources = new HashSet<Award>();
            CompanyPackages = new HashSet<CompanyPackage>();
            RegisterationRequests = new HashSet<RegisterationRequest>();
            SocioEonomicSources = new HashSet<SocioEonomic>();
            SocioEonomicTypes = new HashSet<SocioEonomic>();
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string LookupCode { get; set; }
        public string Name { get; set; }
        public int? Order { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModiciationDate { get; set; }
        public int? AccountId { get; set; }
        public string Description { get; set; }
        public bool? IsDefault { get; set; }
        public bool? UserDefined { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<Award> AwardOperatorNavigations { get; set; }
        public virtual ICollection<Award> AwardSources { get; set; }
        public virtual ICollection<CompanyPackage> CompanyPackages { get; set; }
        public virtual ICollection<RegisterationRequest> RegisterationRequests { get; set; }
        public virtual ICollection<SocioEonomic> SocioEonomicSources { get; set; }
        public virtual ICollection<SocioEonomic> SocioEonomicTypes { get; set; }
    }
}
