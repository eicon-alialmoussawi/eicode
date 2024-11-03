using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class AboutSpectre
    {
        public int Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string TitleFr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionFr { get; set; }
    }
}
