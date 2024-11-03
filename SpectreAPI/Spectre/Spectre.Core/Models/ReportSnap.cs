using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class ReportSnap
    {
        public int Id { get; set; }
        public string ReferenceText { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
