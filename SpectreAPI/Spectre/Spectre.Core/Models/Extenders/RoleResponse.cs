using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class RoleResponse
    {
        public Nullable<Guid> Id { get; set; }
        public string RoleName { get; set; }
        public string Permissions { get; set; }
    }
}
