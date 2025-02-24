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
    public class BazStateRepository : Repository<BazState>, IBazStateRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazStateRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazState
        public async Task<Tuple<bool, BazState>> Create(BazState BazStateEntity)
        {
            try
            {
                var result = await MyDbContext.BazStates.AddAsync(BazStateEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazState>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazState>(false, null);
            }
        }

        // Get all BazStates
        public async Task<IEnumerable<BazState>> GetAll()
        {
            return await MyDbContext.BazStates.ToListAsync();
        }

        // Get BazState by ID
        public async Task<BazState> GetById(int id)
        {
            return await MyDbContext.BazStates
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
