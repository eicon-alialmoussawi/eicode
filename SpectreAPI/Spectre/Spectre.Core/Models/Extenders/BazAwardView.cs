namespace Spectre.Core.Models.Extenders
{
    public class BazAwardViewForPricing : BazAwardView
    {
        public double GDPc { get; set; }
        public double PPP_Factor { get; set; }
        public double InflationFactor { get; set; }
        public double Dollar_per_MHz_pop { get; set; }
        public double MHz_pop { get; set; }
        public double Adjust_Dollar_per_MHz_pop { get; set; }
        public string AdjustmentError { get; set; }
        public double Adjusted_Price_mUSD { get; set; }



    }
    public class BazAwardView
    {
        public int Id { get; set; }
        public string AuctionNumber { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public string Operator { get; set; }
        public decimal Term_Y { get; set; }
        public string MarketCode { get; set; }
        public string Band { get; set; }
        public string Duplex { get; set; }
        public string Region { get; set; }
        public string County { get; set; }
        public int Block_MHZ { get; set; }
        public string Population { get; set; }
        public double Price_mUSD { get; set; }

    }
}
