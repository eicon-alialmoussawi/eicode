using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Page
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PageUrl { get; set; }
        public string NameAr { get; set; }
        public string NameFr { get; set; }
    }
}
