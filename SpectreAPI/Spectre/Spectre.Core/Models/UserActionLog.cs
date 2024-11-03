using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class UserActionLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? Action { get; set; }
        public string Page { get; set; }
        public string PageAr { get; set; }
        public string Details { get; set; }
        public string DetailsAr { get; set; }
        public DateTime? Date { get; set; }
        public byte[] Stamp { get; set; }

        public virtual User User { get; set; }
    }
}
