using System.Numerics;

namespace Spectre.Core.Models.Extenders
{
    public class BazAwardView
    {
        public int Id { get; set; }
        public string AuctionNumber { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public string Operator { get; set; }
        public decimal Term_Y { get; set; }
        public string Group { get; set; }
        public int Band { get; set; }
        public string BandType { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public int Block_MHZ { get; set; }
        public BigInteger Population { get; set; }
        public float Price_mUSD { get; set; }

    }
}
