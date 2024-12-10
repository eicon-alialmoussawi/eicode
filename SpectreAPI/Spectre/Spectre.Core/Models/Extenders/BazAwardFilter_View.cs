using System.Collections.Generic;

namespace Spectre.Core.Models.Extenders
{
    public class BazAwardFilter_View
    {
        public List<int> AuctionIds { get; set; }
        public List<int> Regions { get; set; }
        public List<int> Counties { get; set; }
        public List<int> States { get; set; }
    }
}
