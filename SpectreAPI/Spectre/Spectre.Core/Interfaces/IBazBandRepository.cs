using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazBandRepository
    {
        Task<IEnumerable<BazBand>> GetAll();
        Task<BazBand> GetById(int id);
        Task<Tuple<bool, BazBand>> Create(BazBand _operator);
    }
}
