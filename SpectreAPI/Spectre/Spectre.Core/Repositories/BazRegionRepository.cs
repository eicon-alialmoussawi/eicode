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
    public class BazRegionRepository : Repository<BazRegion>, IBazRegionRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazRegionRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazRegion
        public async Task<Tuple<bool, BazRegion>> Create(BazRegion BazRegionEntity)
        {
            try
            {
                var result = await MyDbContext.BazRegions.AddAsync(BazRegionEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazRegion>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazRegion>(false, null);
            }
        }

        // Get all BazRegions
        public async Task<IEnumerable<BazRegion>> GetAll()
        {
            return await MyDbContext.BazRegions.ToListAsync();
        }

        // Get BazRegion by ID
        public async Task<BazRegion> GetById(int id)
        {
            return await MyDbContext.BazRegions
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
