using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Tuple<bool, bool>> SaveRolePermissions(Guid RoleId, string Permissions);
        Task<Tuple<bool, RoleResponse>> GetRoleById(Guid RoleId);
        Task<Tuple<bool, bool>> DeleteRolePermissions(string RoleId);
    }
}
