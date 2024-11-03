using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Interfaces
{
    public interface ISavedFilterRepository
    {
        Task<Tuple<bool, bool>> SaveUserFilters(List<SavedFilters_View> view);
        Task<Tuple<bool, List<SavedFilters_View>>> GetUsersSavedFilters(string PageUrl, int UserId);
        Task<Tuple<bool, List<SavedFilters_View>>> GetUserDefaultSettings(int UserId);
    }
}
