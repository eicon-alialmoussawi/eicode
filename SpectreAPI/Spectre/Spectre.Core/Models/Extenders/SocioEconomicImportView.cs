using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class SocioEconomicImportView
    {
        public int? Year { set; get; }
        public decimal? Value { get; set; }

        public string Iso { set; get; }
        public int SourceId { set; get; }

    }
}
