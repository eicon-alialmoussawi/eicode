using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class TemplateFilterDetail
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public string ControlType { get; set; }
        public string ControlValue { get; set; }
        public string ControlName { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual TemplateFilter Template { get; set; }
    }
}
