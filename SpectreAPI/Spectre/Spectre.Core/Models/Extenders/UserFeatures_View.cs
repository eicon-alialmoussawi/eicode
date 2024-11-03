using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class UserFeatures_View
    {
        public string PageNameEn { get; set; }
        public string PageNameAr { get; set; }
        public string PageNameFr { get; set; }
        public bool? HasLimit { get; set; }
        public string Restriction { get; set; }
        public string PageUrl { get; set; }
    }
}
