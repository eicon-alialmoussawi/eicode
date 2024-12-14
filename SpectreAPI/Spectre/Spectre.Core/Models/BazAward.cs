namespace Spectre.Core.Models
{
    public class BazAward
    {
        public int Id { get; set; }
        public int year { get; set; }
        public string month { get; set; }
        public decimal Term_Y { get; set; }
        public string Group { get; set; }
        public int BANDS { get; set; }
        public int Pop { get; set; }
        public int Block_MHz { get; set; }
        public float Price_mUSD { get; set; }
        public int AuctionId { get; set; }
        public int RegionId { get; set; }
        public int StateId { get; set; }
        public int CountyId { get; set; }
        public int OperatorId { get; set; }
    }
}
