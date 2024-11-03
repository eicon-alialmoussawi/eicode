using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class PricingFilterBody
    {
        public bool IsPPP { set; get; }
        public bool ISIMF { set; get; }
        public bool IsPaired { set; get; }
        public bool IsPairedAndUnPaired { set; get; }
        public bool RegionalLicense { set; get; }
        public int FromYear { set; get; }
        public int ToYear { set; get; }
        public int MaxGDP { set; get; }
        public int MinGDP { set; get; }
        public string CountryIds { set; get; }
        public string Band { set; get; }

        public int SourceId { set; get; }
        public int IssueDate { set; get; }
        public double Term { set; get; }
        public double DiscountRate { set; get; }
        public bool IsIncludeAnnual { set; get; }
        public string SumBand { set; get; }
        public bool UniqueAwards { set; get; }
        public bool AverageAwards { set; get; }
        public bool AnnualizePrice { set; get; }
        public bool AdjustByPPPFactor { set; get; }
        public bool AdjustByInflationFactor { set; get; }

        public bool AdjustByGDPFactor { set; get; }
        public bool AverageSumPricesAndMHZ { set; get; }
        public bool HasRegression { set; get; }
        public int Regression { set; get; }
        public bool IsUnPaired { set; get; }
        public bool IsMultiple { set; get; }

        public bool IsSingle { set; get; }
        public bool HasPercentile { set; get; }
        public double? UpperPercentile { set; get; }
        public double? LowerPercentile { set; get; }
        public bool HasQuartile { set; get; }
        public double KValue { set; get; }
        public bool HasStandardDeviation { set; get; }
        public bool? AutoFiltering { set; get; }
        public double StandardDeviationValue { set; get; }
        public bool? ShowMarkers { set; get; }
        public string Lang { get; set; }
    }
}
