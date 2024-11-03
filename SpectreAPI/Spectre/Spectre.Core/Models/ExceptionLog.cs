using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class ExceptionLog
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public string InnerMessage { get; set; }
        public DateTime ExceptionDate { get; set; }
    }
}
