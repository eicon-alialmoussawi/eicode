using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Package_View
    {
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
        public int? Order { get; set; }


        public Nullable<int> FromYearLimit { get; set; }
        public Nullable<int> ToYearLimit { get; set; }
        public List<PackagePermission_Extender> PagePermissions { get; set; }
    }
}
