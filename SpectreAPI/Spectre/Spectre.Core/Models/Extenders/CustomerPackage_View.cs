using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class CustomerPackage_View
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int? Total { get; set; }
    }
}
