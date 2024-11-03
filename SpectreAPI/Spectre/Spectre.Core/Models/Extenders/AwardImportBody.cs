using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class AwardImportBody
    {
        public int? Year { set; get; }
        public string Month { set; get; }
        public string? Country { set; get; }
        public string? ISO { set; get; }
        public string? Operator { set; get; }
        public decimal? UpFrontFees { set; get; }
        public decimal? AnnualFees { set; get; }
        public string Terms { set; get; }
        public string BandPaired { set; get; }
        public string BandUnPaired { set; get; }
        public string BlockUnPaired { set; get; }
        public string BlockPaired { set; get; }
        public int? RegionalLicense { set; get; }
        public decimal? Pop { set; get; }
        public string IsSingleOrMulti  {set;get; }
        public decimal? ReservePrice { set; get; }
        public string? AuctionDateMonth { set; get; }
        public int? AuctionDateYear { set; get; }

    }
}
