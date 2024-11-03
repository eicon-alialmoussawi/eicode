using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class ActionLogOperation
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public byte[] Stamp { get; set; }
    }
}
