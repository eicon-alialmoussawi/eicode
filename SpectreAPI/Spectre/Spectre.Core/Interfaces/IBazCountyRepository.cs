using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazCountyRepository
    {
        Task<IEnumerable<BazCounty>> GetAll();
        Task<BazCounty> GetById(int id);
        Task<Tuple<bool, BazCounty>> Create(BazCounty _operator);
    }
}
