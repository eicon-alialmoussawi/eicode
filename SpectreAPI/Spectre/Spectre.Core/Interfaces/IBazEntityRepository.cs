using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazEntityRepository
    {
        Task<IEnumerable<BazEntity>> GetAll();
        Task<BazEntity> GetById(int id);
        Task<Tuple<bool, BazEntity>> Create(BazEntity entity);
    }
}
