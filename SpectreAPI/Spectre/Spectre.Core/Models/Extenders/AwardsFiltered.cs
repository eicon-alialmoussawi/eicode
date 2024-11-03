using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class AwardsFiltered
    {

        public string CountryName { set; get; }
        public int? CountryId { set; get; }
        public string OperatorName { set; get; }
        public int? Year { set; get; }
        public int? Month { set; get; }
        public int? AuctionDateMonth { set; get; }
        public int? AuctionDateYear { set; get; }
        public double? AnnualFees { set; get; }
        public double? UpFrontFees { set; get; }

        public double? Price {set;get; }

        public double? PriceForFilter { set; get; }
        public double? PriceM { set; get; }
        public decimal? ReservePrice { set; get; }
        public string Terms { set; get; }
        public string Band { set; get; }
        public string Pairing { set; get; }
        public string MHZ { set; get; }
        public double? Pop { set; get; }
        public double? AwardPop { set; get; }
        public decimal? GDP { set; get; }
        public bool Regionalicense { set; get; }
        public decimal Paid { set; get; }
        public string coverage { set; get; }
        public char SingleOrMultiBand { set; get; }
        public string BandPaired { set; get; }

        public string BandUnPaired { set; get; }
        public int Id { set; get; }
        public double InflationFactor { set; get; }
        public double PPPFactor { set; get; }
        public double GDPValue { set; get; }
        public string BlockPaired { set; get; }
        public string BlockUnPaired { set; get; }

        public double? CalculatedMHZ { set; get; }
        public  string bandCountry { set; get; }

        public int? NumberOfAwards { set; get; }

        public bool IsHidden { set; get; }
        public double? UpperValue { set; get; }
        public double? LowerValue { set; get; }

        public double? aValue { set; get; }
        public double? bValue { set; get; }
    }
}
