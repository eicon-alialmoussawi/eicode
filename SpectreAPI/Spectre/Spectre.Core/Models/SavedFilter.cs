using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class SavedFilter
    {
        public int Id { get; set; }
        public string PageUrl { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
