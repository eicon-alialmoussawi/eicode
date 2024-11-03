using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
public    class PricingFiltered
    {
        public int AwardId { set; get; }
        public string CountryName { set; get; }
        public string OperatorName { set; get; }
        public int Year { set; get; }
        public decimal PricePaid { set; get; }
        public string Terms { set; get; }
        public string Band { set; get; }
        public string Pairing { set; get; }
        public decimal MHZ { set; get; }
        public decimal Pop { set; get; }
        public double GDP { set; get; }
        public bool Regionalicense { set; get; }
        public decimal Paid { set; get; }
        public string coverage { set; get; }
        public string BandCountry { set; get; }
        public double PricePaidFiltered { set; get; }
    }
}
