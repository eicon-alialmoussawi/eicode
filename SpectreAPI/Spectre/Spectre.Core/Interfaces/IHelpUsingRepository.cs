using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IHelpUsingRepository
    {
        Task<IEnumerable<HelpUsing>> GetAll();
        Task<Tuple<bool>> Save(HelpUsing HelpUsing);
        Task<Tuple<bool>> Delete(int Id);
    }
}
