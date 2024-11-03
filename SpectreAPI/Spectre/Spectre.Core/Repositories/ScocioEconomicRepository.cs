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
    public class ScocioEconomicRepository : Repository<SocioEonomic>, IScocioEconomicRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public ScocioEconomicRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }


        public async Task<Tuple<bool, SocioEonomic>> Create(SocioEonomic SocioEonomic)
        {

            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", SocioEonomic.Id);
                    Params.Add("@Year", SocioEonomic.Year);
                    Params.Add("@Month", SocioEonomic.Month);
                    Params.Add("@CountryId", SocioEonomic.CountryId);
                    Params.Add("@SourceId", SocioEonomic.SourceId);
                    Params.Add("@TypeId", SocioEonomic.TypeId);
                    Params.Add("@CreatedBy", SocioEonomic.CreatedBy);
                    Params.Add("@CreationDate", SocioEonomic.CreationDate);
                    Params.Add("@IsDeleted", SocioEonomic.IsDeleted);
                    Params.Add("@Value", SocioEonomic.Value);
                    var Results = await Connection.ExecuteAsync("SaveSocioEconomic", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, SocioEonomic>(true, SocioEonomic);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, SocioEonomic>(false, null);
            }
        }
        public async Task<Tuple<bool, SocioEonomic>> Update(SocioEonomic SocioEonomic)
        {

            try
            {
                var Result = MyDbContext.SocioEonomics.Update(SocioEonomic);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(SocioEonomic).State = EntityState.Detached;

                return new Tuple<bool, SocioEonomic>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, SocioEonomic>(false, null);
            }
        }
        public async Task<SocioEonomic> GetById(int Id)
        {
            return await MyDbContext.SocioEonomics.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }
        public async Task<SocioEonomic> GetByCountryAndYear(int CountryId, int Year, bool ISIMF)
        {
            if (ISIMF)
                return await MyDbContext.SocioEonomics.Where(m => m.CountryId == CountryId && m.Year == Year && m.SourceId == 29).SingleOrDefaultAsync();
            return await MyDbContext.SocioEonomics.Where(m => m.CountryId == CountryId && m.Year == Year && m.SourceId == 14).SingleOrDefaultAsync();

        }


        public async Task<IEnumerable<SocioEonomic>> GetAll()
        {
            return await MyDbContext.SocioEonomics.Where(m => m.IsDeleted == false).ToListAsync();
        }
        public async Task<Tuple<bool, int>> CheckIfExists(SocioEonomic SocioEonomic)
        {

            try
            {
                var res = await MyDbContext.SocioEonomics
                    .AsNoTracking().Where(
                    m => m.IsDeleted == false &&
                    m.Year == SocioEonomic.Year &&
                    m.CountryId == SocioEonomic.CountryId &&
                    m.SourceId == SocioEonomic.SourceId).FirstOrDefaultAsync();

                //  return (res == null ? false : true);
                return Tuple.Create<bool, int>((res == null ? false : true), (res == null ? 0 : res.Id));
            }
            catch(Exception e)
            {
                return Tuple.Create<bool, int>(false, 0);
            }
        
        }


        public async Task<IEnumerable<SoioEconomicColumnHeader>> FilterSocioEconimcsByCountryAndSource(int CountryId, int SourceId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@CountryId", CountryId);
                    Params.Add("@SourceId", SourceId);
                    var Results = await Connection.QueryAsync<SoioEconomicColumnHeader>("FilterSocioEconimcsByCountryAndSource", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }
        public async Task<IEnumerable<SocioEonomic_View2>> FilterSocioEconimcsByCountry(int CountryId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@CountryIds", CountryId);
                    var Results = await Connection.QueryAsync<SocioEonomic_View2>("FilterSocioEconimcsByCountry", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }
        public async Task<IEnumerable<SocioEonomic_View2>> FilterSocioEconimcs(string CountryIds, int SourceId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@CountryIds", CountryIds);
                    Params.Add("@SourceId", SourceId);
                    var Results = await Connection.QueryAsync<SocioEonomic_View2>("FilterSocioeconimcs", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }
        public async Task<IEnumerable<SocioEonomic_View>> GetAllSocioEconomics()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    var Results = await Connection.QueryAsync<SocioEonomic_View>("GetAllSocioEconomics", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }

        public async Task<Tuple<bool>> DeleteAllSocioEcnomic()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    var Results = await Connection.ExecuteAsync("DeleteAllSocioEcnomic", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<Tuple<bool, decimal?>> GetGDPForValuations(int Year, int CountryId, bool IsPPP, bool IsIMF)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Year", Year);
                    Params.Add("@CountryId", CountryId);
                    Params.Add("@IsPPP", IsPPP);
                    Params.Add("@IsIMF", IsIMF);

                    var Results = await Connection.QueryAsync<decimal?>("GetGDPForValuations", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new  Tuple<bool, decimal?>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, decimal?>(false, null);
            }
        }

        public async Task<Tuple<bool, List<SocioEcnomic_YearView>>> FilterSocioEconimcsByYear(int Year, bool? IsIMF)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Year", Year);
                    Params.Add("@IsIMF", IsIMF);

                    var Results = await Connection.QueryAsync<SocioEcnomic_YearView?>("FilterSocioEconimcsByYear", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<SocioEcnomic_YearView>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<SocioEcnomic_YearView>>(false, null);
            }
        }
        public async Task<Tuple<bool, decimal?>> GetPopForValuations(int Year, int CountryId,bool IsIMF)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Year", Year);
                    Params.Add("@CountryId", CountryId);
                    Params.Add("@IsIMF", IsIMF);

                    var Results = await Connection.QueryAsync<decimal?>("GetPopForValuations", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, decimal?>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, decimal?>(false, null);
            }
        }

    }
}
