using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class CompanyPackageDetail
    {
        public int Id { get; set; }
        public int? CompanyPackageId { get; set; }
        public int? PackagePagePermissionId { get; set; }
        public int? CountryId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual CompanyPackage CompanyPackage { get; set; }
        public virtual Country Country { get; set; }
        public virtual PackagePagePermission PackagePagePermission { get; set; }
    }
}
