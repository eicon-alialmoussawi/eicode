using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class AwardFilter_View
    {
        public string CountryIds { get; set; }
        public string BandTypes { get; set; }
        public string OperatorName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string BandUnPaired { get; set; }

    }
}
