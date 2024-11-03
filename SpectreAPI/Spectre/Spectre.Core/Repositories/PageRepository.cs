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
    public class PageRepository : Repository<Page>, IPageRepository
    {
        private readonly ILogger ILogger;
        private readonly IConfiguration configuration;
        public PageRepository(SpectreDBContext context, ILogger ILogger, IConfiguration configuration)
            : base(context)
        {
            this.ILogger = ILogger;
            this.configuration = configuration;

        }

        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }


        public async Task<Tuple<bool, List<Page>>> GetAll()
        {
            try
            {
                List<Page> myPages = await MyDbContext.Pages.ToListAsync();
                return new Tuple<bool, List<Page>>(true, myPages);
            }
            catch(Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, List<Page>>(false, null);
            }
         
        }
    }
}
