using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Country_View
    {
        public int CountryId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Iso3 { get; set; }
        public int RegionId { get; set; }
    }
}
