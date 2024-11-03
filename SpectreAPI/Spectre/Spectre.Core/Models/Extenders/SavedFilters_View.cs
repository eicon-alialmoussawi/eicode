using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class SavedFilters_View
    {
        public int Id { get; set; }
        public string PageUrl { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public int? UserId { get; set; }
    }
}
