using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class TemplateFilter
    {
        public TemplateFilter()
        {
            TemplateFilterDetails = new HashSet<TemplateFilterDetail>();
        }

        public int Id { get; set; }
        public string PageName { get; set; }
        public string Title { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual ICollection<TemplateFilterDetail> TemplateFilterDetails { get; set; }
    }
}
