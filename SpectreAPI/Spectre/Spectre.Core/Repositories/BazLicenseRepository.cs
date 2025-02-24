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
    public class BazLicenseRepository : Repository<BazLicense>, IBazLicenseRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BazLicenseRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new BazLicense
        public async Task<Tuple<bool, BazLicense>> Create(BazLicense BazLicenseEntity)
        {
            try
            {
                var result = await MyDbContext.BazLicense.AddAsync(BazLicenseEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, BazLicense>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, BazLicense>(false, null);
            }
        }

        // Get all BazLicenses
        public async Task<IEnumerable<BazLicense>> GetAll()
        {
            return await MyDbContext.BazLicense.ToListAsync();
        }

        // Get BazLicense by ID
        public async Task<BazLicense> GetById(int id)
        {
            return await MyDbContext.BazLicense
                                     .Where(o => o.Id == id)
                                     .SingleOrDefaultAsync();
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

    }
}
