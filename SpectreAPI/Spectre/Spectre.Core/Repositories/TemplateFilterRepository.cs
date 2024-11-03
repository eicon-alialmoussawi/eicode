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
    public class TemplateFilterRepository

  : Repository<RegisterationRequest>, ITemplateFilterRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public TemplateFilterRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, TemplateFilterDetail>> AddFilterDetails(TemplateFilterDetail TemplateFilterDetail)
        {
            try
            {
                var Result = await MyDbContext.TemplateFilterDetails.AddAsync(TemplateFilterDetail);
                MyDbContext.SaveChanges();

                return new Tuple<bool, TemplateFilterDetail>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, TemplateFilterDetail>(false, null);
            }
        }

        public async Task<Tuple<bool, TemplateFilter>> Create(TemplateFilter TemplateFilter)
        {
            try
            {
                var Result = await MyDbContext.TemplateFilters.AddAsync(TemplateFilter);
                MyDbContext.SaveChanges();

                return new Tuple<bool, TemplateFilter>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, TemplateFilter>(false, null);
            }
        }

        public async Task<IEnumerable<TemplateFilter>> GetAll()
        {

            return await MyDbContext.TemplateFilters.Where(m => m.IsDeleted == false).ToListAsync();
        }
        public async Task<IEnumerable<TemplateFilter>> GetAllByPage(string Page)
        {

            return await MyDbContext.TemplateFilters.Where(m => m.IsDeleted == false && m.PageName==Page).ToListAsync();
        }
        

        public async Task<TemplateFilter> GetById(int Id)
        {
            return await MyDbContext.TemplateFilters.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<TemplateFilterDetail>> GetDetailsByFilterId(int TemplateFilterId)
        {
            return await MyDbContext.TemplateFilterDetails.Where(m => m.IsDeleted == false && m.TemplateId==TemplateFilterId).ToListAsync();

        }

        public async Task<Tuple<bool, TemplateFilter>> Update(TemplateFilter TemplateFilter)
        {
            try
            {
                var Result = MyDbContext.TemplateFilters.Update(TemplateFilter);
                MyDbContext.SaveChanges();

                return new Tuple<bool, TemplateFilter>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, TemplateFilter>(false, null);
            }
        }
    }
}
