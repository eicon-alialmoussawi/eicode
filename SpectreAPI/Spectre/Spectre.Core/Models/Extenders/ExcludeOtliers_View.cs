using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class ExcludeOtliers_View
    {
        public bool? HasPercentile { get; set; }
        public bool? HasQuartile { get; set; }
        public bool? HasStandardDeviation { get; set; }
        public bool? HasRegression { get; set; }
        public bool? AutoFiltering { get; set; }
        public double? KValue { set; get; }
        public double? UpperPercentile { set; get; }
        public double? LowerPercentile { set; get; }
        public double? StandardDeviationValue { set; get; }
        public double? Regression { set; get; }
        public bool? ShowMarkers { set; get; }
    }
}
