using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class CompanyPackageDetails_View2
    {
        public int Id { get; set; }
        public int? CompanyPackageId { get; set; }
        public int? PackagePagePermissionId { get; set; }
        public int CountryId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
