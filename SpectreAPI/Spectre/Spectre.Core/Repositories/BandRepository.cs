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
    public class BandRepository : Repository<Band>, IBandRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public BandRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, Band>> Create(Band Band)
        {

            try
            {
                var Result = await MyDbContext.Bands.AddAsync(Band);
                MyDbContext.SaveChanges();

                return new Tuple<bool, Band>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, Band>(false, null);
            }
        }

        public async Task<Tuple<bool>> DeleteBand(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);


                    var Results = await Connection.ExecuteAsync("DeleteBand", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool>(true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool>(false);
            }
        }

        public async Task<IEnumerable<Band>> GetAll()
        {
            return await MyDbContext.Bands.Where(m => m.IsDeleted == false).OrderBy(m => m.Value).ToListAsync();
        }

        public async Task<Band> GetById(int Id)
        {
            return await MyDbContext.Bands.AsNoTracking().Where(m => m.Id == Id).SingleOrDefaultAsync();
        }

        public async Task<Tuple<bool>> RemoveAllBands()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.ExecuteAsync("RemoveAllBands", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool>(true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool>(false);
            }
        }

        public async Task<Tuple<bool, bool>> SaveBand(Band Band)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Band.Id);
                    Params.Add("@Value", Band.Value);
                    Params.Add("@TitleAr", Band.TitleAr);
                    Params.Add("@TitleEn", Band.TitleEn);
                    Params.Add("@IsSelected", Band.IsSelected);
                    Params.Add("@IsDeleted", Band.IsDeleted);
                    Params.Add("@CreationDate", Band.CreationDate);
                    Params.Add("@CreatedBy", Band.CreatedBy);

                    var Results = await Connection.QueryAsync<bool>("SaveBands", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, bool>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }

        public async Task<Tuple<bool, Band>> Update(Band Band)
        {
            try
            {
                var Result = MyDbContext.Bands.Update(Band);
                MyDbContext.SaveChanges();

                return new Tuple<bool, Band>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, Band>(false, null);
            }
        }
    }
}
