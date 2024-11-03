using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class CompanyPackage_View
    {
        public int PreRegistrationId { get; set; }
        public Company company { get; set; }
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
        public List<CompanyPackageDetails_View> companyPackageDetails { get; set; }
        public List<User_View> users { get; set; }

    }
}
