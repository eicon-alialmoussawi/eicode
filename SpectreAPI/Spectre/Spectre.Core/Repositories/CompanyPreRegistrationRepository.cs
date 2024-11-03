using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;

namespace Spectre.Core.Repositories
{
    public class CompanyPreRegistrationRepository : Repository<CompanyPreRegistration>, ICompanyPreRegistrationRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public CompanyPreRegistrationRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, CompanyPreRegistration>> Create(CompanyPreRegistration PreRegistration)
        {
            try
            {
                var Result = await MyDbContext.CompanyPreRegistrations.AddAsync(PreRegistration);
                MyDbContext.SaveChanges();

                return new Tuple<bool, CompanyPreRegistration>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, CompanyPreRegistration>(false, null);
            }
        }

        public async Task<Tuple<bool>> DeletePreRegistration(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.ExecuteAsync("DeletePreRegistration", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<IEnumerable<CompanyPreRegistration>> GetAll()
        {
            return await MyDbContext.CompanyPreRegistrations.Where(m => m.IsDeleted == false).ToListAsync();
        }

        public async Task<Tuple<bool, List<CompanyPreRegistration_Details>>> GetAllDetails()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.QueryAsync<CompanyPreRegistration_Details>("GetAllPreRegistrationDetails", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<CompanyPreRegistration_Details>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<CompanyPreRegistration_Details>>(false, null);
            }
        }

        public async Task<Tuple<bool, CompanyPreRegistration_Details>> GetPreRegistrationDetailsById(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.QueryAsync<CompanyPreRegistration_Details>("GetPreRegistrationDetailsById", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, CompanyPreRegistration_Details>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, CompanyPreRegistration_Details>(false, null);
            }
        }

        public async Task<Tuple<bool>> SetPreRegistrationAsViewed(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.ExecuteAsync("SetPreRegistrationAsViewed", Params, commandType: CommandType.StoredProcedure);
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
    }
}
