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
    public class CountryRepository
   : Repository<Country>, ICountryRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public CountryRepository(IConfiguration configuration, SpectreDBContext context, ILogger Logger)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        public async Task<Tuple<bool, Country>> Create(Country Country)
        {

            try
            {
                var Result = await MyDbContext.Countries.AddAsync(Country);
                MyDbContext.SaveChanges();

                return new Tuple<bool, Country>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, Country>(false, null);
            }
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<IEnumerable<Country>> GetAll()
        {
            return await MyDbContext.Countries.ToListAsync();
        }

        public  async Task<Country> GetByCode(string Code)
        {
              return await MyDbContext.Countries.Where(m => m.Iso3 == Code).SingleOrDefaultAsync();
        }

        public async Task<Country> GetById(int Id)
        {
            return await MyDbContext.Countries.Where(m => m.CountryId==Id).SingleOrDefaultAsync();
        }
        public async Task<Country> getByIso3(string Code)
        {
            return await MyDbContext.Countries.Where(m => m.Iso3 == Code).SingleOrDefaultAsync();
        }

        public async Task<Tuple<bool, List<Country_View>>> GetUserCountries(int UserId, string PageUrl, string Source, string Lang)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);
                    Params.Add("@PageUrl", PageUrl);
                    Params.Add("@Source", Source);
                    Params.Add("@Lang", Lang);



                    var Results = await Connection.QueryAsync<Country_View>("GetUserCountries", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<Country_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<Country_View>>(false, null);
            }
        }
    }
}
