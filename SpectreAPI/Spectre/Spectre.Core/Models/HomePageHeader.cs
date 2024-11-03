using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class HomePageHeader
    {
        public int Id { get; set; }
        public string Title1En { get; set; }
        public string Title1Ar { get; set; }
        public string Title1Fr { get; set; }
        public string Title2En { get; set; }
        public string Title2Ar { get; set; }
        public string Title2Fr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionFr { get; set; }
    }
}
