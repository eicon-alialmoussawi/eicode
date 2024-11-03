using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class CompanyPackageD_View
    {
        public int CompanyId { get; set; }
        public int CompanyPackageId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PackageName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
