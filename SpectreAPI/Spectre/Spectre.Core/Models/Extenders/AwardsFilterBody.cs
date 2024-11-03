using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class AwardsFilterBody
    {
        public bool ISPPP { set; get; }
        public bool ISIMF { set; get; }
        public bool IsSingle { set; get; }
        public bool IsMultiple { set; get; }
        public bool IsPaired { set; get; }
        public bool IsUnPaired { set; get; }
        public bool IsPairedAndUnPaired { set; get; }
        public bool RegionalLicense { set; get; }
        public int FromYear { set; get; }
        public int ToYear { set; get; }
        public int MaxGDP { set; get; }
        public int MinGDP { set; get; }
        public string CountryIds { set; get; }
        public string Band { set; get; }
        public int SourceId { set; get; }
        public string Lang { set; get; }

    }
}
