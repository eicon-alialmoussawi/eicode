using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class RolePermission
    {
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        public int PermissionId { get; set; }

        public virtual AspnetPermission Permission { get; set; }
    }
}
