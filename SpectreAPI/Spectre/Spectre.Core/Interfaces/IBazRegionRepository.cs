using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazRegionRepository
    {
        Task<IEnumerable<BazRegion>> GetAll();
        Task<BazRegion> GetById(int id);
        Task<Tuple<bool, BazRegion>> Create(BazRegion _operator);
    }
}
