using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazMarketClassRepository
    {
        Task<IEnumerable<BazMarketClass>> GetAll();
        Task<BazMarketClass> GetById(int id);
        Task<Tuple<bool, BazMarketClass>> Create(BazMarketClass _market);
    }
}
