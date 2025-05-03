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
    public class BazBandRepository : Repository<BazBand>, IBazBandRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazBandRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazBand
        public async Task<Tuple<bool, BazBand>> Create(BazBand BazBandEntity)
        {
            try
            {
                var result = await MyDbContext.BazBands.AddAsync(BazBandEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazBand>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazBand>(false, null);
            }
        }

        // Get all BazBands
        public async Task<IEnumerable<BazBand>> GetAll()
        {
            return await MyDbContext.BazBands.ToListAsync();
        }

        // Get BazBand by ID
        public async Task<BazBand> GetById(int id)
        {
            return await MyDbContext.BazBands
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
