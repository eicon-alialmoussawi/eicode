using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.RepositoryHandler
{
    public interface ILogger
    {
        Task LogException(Exception ex);
        Task LogCustomException(string Source, string Message);
        Task LogUserAction(UserActionLog userActionLog);
        Task<IEnumerable<ExceptionLog>> GetExceptionLogs();

        Task<Tuple<bool, List<LoginDetails>>> GetLoginDetails(DateTime date, int userId);
    }
}
