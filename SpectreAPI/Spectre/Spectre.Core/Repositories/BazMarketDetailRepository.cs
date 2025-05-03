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
    public class BazMarketDetailRepository : Repository<BazMarketDetail>, IBazMarketDetailRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazMarketDetailRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazMarketDetail
        public async Task<Tuple<bool, BazMarketDetail>> Create(BazMarketDetail BazMarketDetailEntity)
        {
            try
            {
                var result = await MyDbContext.BazMarketDetails.AddAsync(BazMarketDetailEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazMarketDetail>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazMarketDetail>(false, null);
            }
        }

        // Get all BazMarketDetails
        public async Task<IEnumerable<BazMarketDetail>> GetAll()
        {
            return await MyDbContext.BazMarketDetails.ToListAsync();
        }

        // Get BazMarketDetail by ID
        public async Task<BazMarketDetail> GetById(int id)
        {
            return await MyDbContext.BazMarketDetails
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
