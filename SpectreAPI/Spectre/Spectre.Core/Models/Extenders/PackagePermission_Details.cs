using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class PackagePermission_Details
    {
        public int Id { get; set; }
        public int? PackageId { get; set; }
        public string PageUrl { get; set; }
        public bool? HasCountryLimit { get; set; }
        public bool? IsDeleted { get; set; }
        public string PageName { get; set; }
    }
}
