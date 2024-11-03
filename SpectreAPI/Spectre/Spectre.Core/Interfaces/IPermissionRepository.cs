using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface  IPermissionRepository
    {
        Task<Tuple<bool, List<AspnetPermission>>> GetUserPermissions(string IdentityUserId, string Action, string PageUrl);
        Task<Tuple<bool, List<AspnetPermission>>> GetAllPermissions();
        Task<Tuple<bool, bool>> CheckIfUserCanView(int UserId, string PageUrl);
    }
}
