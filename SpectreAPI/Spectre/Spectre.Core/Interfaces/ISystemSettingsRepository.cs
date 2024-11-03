using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Spectre.Core.Interfaces
{
    public interface ISystemSettingsRepository
    {
        Task<SystemSetting> GetById(int Id);
        Task<IEnumerable<SystemSetting>> GetAll();

        Task<Tuple<bool, SystemSetting>> Create(SystemSetting SystemSetting);
        Task<Tuple<bool, SystemSetting>> Update(SystemSetting SystemSetting);

    }
}
