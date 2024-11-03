using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Benchmark_View
    {
        public string Band { get; set; }
        public double Avalue { get; set; }
        public double Bvalue { get; set; }
        public int NumberOfAwards { get; set; }
        public double ssreg { get; set; }
        public double ssot { get; set; }
        public double RSQ { get; set; }
        public double mean { get; set; }
        public double median { get; set; }

    }
}
