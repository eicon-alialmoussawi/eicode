using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class SocioEonomic_View
    {
        public int Id { set; get; }
        public int Year { set; get; }
        public int Month { set; get; }
        public string CountryName { set; get; }
        public string Source { set; get; }
        public string Type { set; get; }
        public DateTime? CreationDate { set; get; }
    }
    
}
