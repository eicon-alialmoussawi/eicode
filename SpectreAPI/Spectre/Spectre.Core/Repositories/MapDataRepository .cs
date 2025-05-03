using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class MapDataRepository : Repository<MapData>, IMapDataRepository
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public MapDataRepository(SpectreDBContext context, ILogger logger, IConfiguration configuration)
            : base(context)
        {
            this.logger = logger;
            this.configuration = configuration;
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }


        public async Task<string> GetJsonByName(string name)
        {
            try
            {
                var result = await MyDbContext.MapDatas
                                .Where(m => m.Name == name)
                                .Select(m => m.JsonContent)
                                .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                await logger.LogException(ex);
                return null;
            }
        }
    }

}
