using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class AdjustmentModel_View
    {
        public bool IsIncludeAnnual { get; set; }
        public double DiscountRate { set; get; }
        public bool ISIMF { get; set; }
        public bool AdjustByInflationFactor { get; set; }
        public bool AdjustByPPPFactor { get; set; }
        public bool AnnualizePrice { get; set; }
        public double Term { get; set; }
        public bool AdjustByGDPFactor { get; set; }
        public bool UniqueAwards { get; set; }
        public string SumBand { get; set; }

    }
}
