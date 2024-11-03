using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Regression_Recalculate
    {
        public List<AwardsFiltered> AwardsFiltered { get; set; }
        public int? CountryId { get; set; }
        public double? PopCovered { get; set; }
        public string Lang { get; set; }
        public string EnforeBPositive { get; set; }
        public int IssueDate { set; get; }
        public bool ISIMF { get; set; }
        public bool IsPPP { get; set; }

        public string Terms { get; set; }
        public string Band { get; set; }
    }
}
