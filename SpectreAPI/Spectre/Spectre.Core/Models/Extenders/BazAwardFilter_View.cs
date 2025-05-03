using System.Collections.Generic;

namespace Spectre.Core.Models.Extenders
{
    public class BazAwardFilter_ViewForPricing : BazAwardFilter_View
    {
        public int IssueDate { get; set; }
        public float MinGDPc { get; set; }
        public float MaxGDPc { get; set; }
        public int MinPopulation { get; set; }
        public int MaxPopulation { get; set; }
        public bool? AdjustForInflation { get; set; }
        public double Term { set; get; }
        public double DiscountRate { set; get; }
    }
    public class BazAwardFilter_View
    {
        public List<int> AuctionIds { get; set; }
        public List<int> MarketCodeIds { get; set; }
        public List<int> MarketClassIds { get; set; }
        public List<int> BandIds { get; set; }
        public List<int> Licenses { get; set; }
    }
}
