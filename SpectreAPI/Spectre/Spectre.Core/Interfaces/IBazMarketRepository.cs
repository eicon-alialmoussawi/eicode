using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazMarketRepository
    {
        Task<IEnumerable<BazMarket>> GetAll();
        Task<BazMarket> GetById(int id);
        Task<Tuple<bool, BazMarket>> Create(BazMarket _market);
    }
}
