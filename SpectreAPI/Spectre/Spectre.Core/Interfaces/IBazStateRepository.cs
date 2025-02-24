using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazStateRepository
    {
        Task<IEnumerable<BazState>> GetAll();
        Task<BazState> GetById(int id);
        Task<Tuple<bool, BazState>> Create(BazState _operator);
    }
}
