using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;

namespace Spectre.Core.Interfaces
{
    public interface IFQARepository
    {
        Task<IEnumerable<Fqa>> GetAll();
        Task<Tuple<bool>> Save(Fqa Fqa);
        Task<Tuple<bool>> Delete(int Id);
    }
}
