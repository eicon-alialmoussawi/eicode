using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Band
    {
        public int Id { get; set; }
        public int? Value { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public bool? IsSelected { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; }
    }
}
