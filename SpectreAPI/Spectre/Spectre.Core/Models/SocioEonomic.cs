using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class SocioEonomic
    {
        public int Id { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? CountryId { get; set; }
        public int? SourceId { get; set; }
        public int? TypeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? Value { get; set; }

        public virtual Country Country { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual Lookup Source { get; set; }
        public virtual Lookup Type { get; set; }
    }
}
