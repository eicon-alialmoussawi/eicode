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
    public class BazMarketClassRepository : Repository<BazMarketClass>, IBazMarketClassRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazMarketClassRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazMarketClass
        public async Task<Tuple<bool, BazMarketClass>> Create(BazMarketClass BazMarketClassEntity)
        {
            try
            {
                var result = await MyDbContext.MarketClass.AddAsync(BazMarketClassEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazMarketClass>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazMarketClass>(false, null);
            }
        }

        // Get all BazMarketClasss
        public async Task<IEnumerable<BazMarketClass>> GetAll()
        {
            return await MyDbContext.MarketClass.ToListAsync();
        }

        // Get BazMarketClass by ID
        public async Task<BazMarketClass> GetById(int id)
        {
            return await MyDbContext.MarketClass
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
