using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class BazAuctionRepository : Repository<BazAuction>, IBazAuctionRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazAuctionRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazAuction
        public async Task<Tuple<bool, BazAuction>> Create(BazAuction BazAuctionEntity)
        {
            try
            {
                var result = await MyDbContext.BazAuctions.AddAsync(BazAuctionEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazAuction>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazAuction>(false, null);
            }
        }

        // Get all BazAuctions
        public async Task<IEnumerable<BazAuction>> GetAll()
        {
            return await MyDbContext.BazAuctions.ToListAsync();
        }

        // Get BazAuction by ID
        public async Task<BazAuction> GetById(int id)
        {
            return await MyDbContext.BazAuctions
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
