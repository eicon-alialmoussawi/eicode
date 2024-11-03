using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Distancing_View
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public int? CountryId { get; set; }
        public string LowBand { get; set; }
        public int? LowBandYear { get; set; }
        public double? LowBandPrice { get; set; }
        public string HighBand { get; set; }
        public int? HighBandYear { get; set; }
        public double? HighBandPrice { get; set; }
        public string TargetBand { get; set; }
        public int? TargetBandYear { get; set; }
        public double? TargetBandPrice { get; set; }
        public double? RelativePrice { get; set; }


        public string ValuatedLowBand { get; set; }
        public int? ValuatedLowBandYear { get; set; }
        public double? ValuatedLowBandPrice { get; set; }


        public string ValuatedHighBand { get; set; }
        public int? ValuatedHighBandYear { get; set; }
        public double? ValuatedHighBandPrice { get; set; }
    }
}
