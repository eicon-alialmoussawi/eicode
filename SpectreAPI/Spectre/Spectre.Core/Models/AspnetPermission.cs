using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class AspnetPermission
    {
        public AspnetPermission()
        {
            RolePermissions = new HashSet<RolePermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PageUrl { get; set; }
        public string Action { get; set; }
        public string ActionAr { get; set; }
        public bool? ShowMenu { get; set; }
        public int? MenuOrder { get; set; }
        public bool Deleted { get; set; }
        public string GroupNameEn { get; set; }
        public string GroupNameAr { get; set; }
        public string MenuNameEn { get; set; }
        public string MenuNameAr { get; set; }
        public string IconClass { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
