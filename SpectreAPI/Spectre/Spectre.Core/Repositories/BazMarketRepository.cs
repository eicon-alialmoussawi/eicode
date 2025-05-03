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
    public class BazMarketRepository : Repository<BazMarket>, IBazMarketRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazMarketRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazMarket
        public async Task<Tuple<bool, BazMarket>> Create(BazMarket BazMarketEntity)
        {
            try
            {
                var result = await MyDbContext.BazMarkets.AddAsync(BazMarketEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazMarket>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazMarket>(false, null);
            }
        }

        // Get all BazMarkets
        public async Task<IEnumerable<BazMarket>> GetAll()
        {
            return await MyDbContext.BazMarkets.ToListAsync();
        }

        // Get BazMarket by ID
        public async Task<BazMarket> GetById(int id)
        {
            return await MyDbContext.BazMarkets
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
