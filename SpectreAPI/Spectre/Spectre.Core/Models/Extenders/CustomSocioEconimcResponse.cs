using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class CustomSocioEconimcResponse
    {
        public string CountryName { set; get; }
        public SoioEconomicColumnHeader[] items { set; get; }   
        public int CountryId { set; get; }
        public string SourceName { set; get; }
    }
}
