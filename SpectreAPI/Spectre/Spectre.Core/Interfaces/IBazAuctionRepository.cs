using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazAuctionRepository
    {
        Task<IEnumerable<BazAuction>> GetAll();
        Task<BazAuction> GetById(int id);
        Task<Tuple<bool, BazAuction>> Create(BazAuction _operator);
    }
}
