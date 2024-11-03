using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models
{
    public class CustomAwardFiltered
    {
        public string Band { set; get; }
        public List<AwardsFiltered> Awards { set; get; }
    }
}
