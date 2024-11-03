using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Region
    {
        public Region()
        {
            Countries = new HashSet<Country>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string NameEn { get; set; }
        public string NameFr { get; set; }
        public string NameAr { get; set; }
        public virtual ICollection<Country> Countries { get; set; }
    }
}
