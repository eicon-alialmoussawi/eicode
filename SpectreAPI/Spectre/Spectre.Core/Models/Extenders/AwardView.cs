using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class AwardView
    {

        public int Id { get; set; }
        public decimal? PricePaid { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CountryName { set; get; }
        public string OperatorName { set; get; }
        public string CreatedBy { set; get; }

        public string BandPaired { set; get; }
        public string BandUnPaired { set; get; }

        public int? Year { get; set; }
    }
}
