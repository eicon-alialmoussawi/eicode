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
    public class BazCountyRepository : Repository<BazCounty>, IBazCountyRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazCountyRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazCounty
        public async Task<Tuple<bool, BazCounty>> Create(BazCounty BazCountyEntity)
        {
            try
            {
                var result = await MyDbContext.BazCounties.AddAsync(BazCountyEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazCounty>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazCounty>(false, null);
            }
        }

        // Get all BazCountys
        public async Task<IEnumerable<BazCounty>> GetAll()
        {
            return await MyDbContext.BazCounties.ToListAsync();
        }

        // Get BazCounty by ID
        public async Task<BazCounty> GetById(int id)
        {
            return await MyDbContext.BazCounties
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
