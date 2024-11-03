using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class FilterModel_View
    {
        public bool RegionalLicense { get; set; }
        public int MinGDP { get; set; }
        public int MaxGDP { get; set; }
        public bool IsPairedAndUnPaired { get; set; }
        public bool IsPaired { get; set; }
        public bool IsUnPaired { get; set; }
        public bool IsSingle { get; set; }
        public string Band { get; set; }

    }
}
