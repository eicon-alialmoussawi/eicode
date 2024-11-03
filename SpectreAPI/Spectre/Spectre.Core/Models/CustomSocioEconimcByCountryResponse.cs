using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models
{
   public class CustomSocioEconimcByCountryResponse
    {
        public string IndicatorName { set; get; }
        public SoioEconomicColumnHeader[] items { set; get; }
    }
}
