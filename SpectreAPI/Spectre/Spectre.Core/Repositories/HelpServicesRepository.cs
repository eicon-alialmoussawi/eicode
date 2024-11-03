using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class HelpServicesRepository : Repository<HelpService>, IHelpServicesRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;

        public HelpServicesRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
         : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }

        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

  

        public async Task<IEnumerable<HelpService>> GetAll()
        {
            return await MyDbContext.HelpServices.ToListAsync();
        }

        public async Task<Tuple<bool>> Save(HelpService HelpService)
        {
            try
            {
                if (HelpService.Id == 0)
                {
                    var Result = await MyDbContext.HelpServices.AddAsync(HelpService);
                    MyDbContext.SaveChanges();

                    MyDbContext.Entry(HelpService).State = EntityState.Detached;
                    return new Tuple<bool>(true);
                }
                else
                {
                    MyDbContext.Entry(HelpService).State = EntityState.Modified;
                    MyDbContext.SaveChanges();
                    return new Tuple<bool>(true);
                }
                
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool>(false);
            }
        }
    }
}
