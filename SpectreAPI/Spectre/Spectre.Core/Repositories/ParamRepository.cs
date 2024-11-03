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
    public class ParamRepository : Repository<Parameter>, IParamRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public ParamRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }

        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, List<Parameter>>> GetAll()
        {
            try
            {
                var Result = await MyDbContext.Parameters.ToListAsync();
                return new Tuple<bool, List<Parameter>>(true, Result);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<Parameter>>(false, null);
            }
        }

        public async Task<Tuple<bool, bool>> Update(int ParamId, string Value)
        {
            try
            {
                var Result = await MyDbContext.Parameters.FirstOrDefaultAsync(c => c.Id == ParamId);
                if (Result != null)
                {
                    Result.Value = Value;
                    MyDbContext.Entry(Result).State = EntityState.Modified;
                    await MyDbContext.SaveChangesAsync();
                }
                return new Tuple<bool, bool>(true, true);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }

        public async Task<Tuple<bool, Parameter>> GetById(int ParamId)
        {
            try
            {
                var Result = await MyDbContext.Parameters.FirstOrDefaultAsync(c => c.Id == ParamId);
                return new Tuple<bool, Parameter>(true, Result);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, Parameter>(false, null);
            }
        }
    }
}
