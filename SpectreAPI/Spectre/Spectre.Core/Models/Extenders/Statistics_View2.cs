using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class Statistics_View2
    {
        public List<Sales_View> sales { get; set; }
        public List<CustomerPackage_View> customers { get; set; }
        public ActivePackage_View active { get; set; }
    }

}
