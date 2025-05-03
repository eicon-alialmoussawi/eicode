using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazMarketDetailRepository
    {
        Task<IEnumerable<BazMarketDetail>> GetAll();
        Task<BazMarketDetail> GetById(int id);
        Task<Tuple<bool, BazMarketDetail>> Create(BazMarketDetail _operator);
    }
}
