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
    public class CompanyPackageRepository : Repository<CompanyPackageRepository>, ICompanyPackageRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public CompanyPackageRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }

        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, List<CompanyPackageD_View>>> GetAllWithDetails()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();

                    var Results = await Connection.QueryAsync<CompanyPackageD_View>("GetCompanyPackages", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<CompanyPackageD_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<CompanyPackageD_View>>(false, null);
            }
        }

        public async Task<Tuple<bool, Company_View>> GetCompanyById(int? CompanyId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", CompanyId);

                    var Results = await Connection.QueryAsync<Company_View>("GetCompanyById", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, Company_View>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, Company_View>(false, null);
            }
        }

        public async Task<Tuple<bool, List<CompanyPackageDetails_View2>>> GetDetails(int CompanyPackageId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@CompanyPackageId", CompanyPackageId);

                    var Results = await Connection.QueryAsync<CompanyPackageDetails_View2>("GetCompanyPackageDetails", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<CompanyPackageDetails_View2>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<CompanyPackageDetails_View2>>(false, null);
            }
        }

        public async Task<Tuple<bool, List<CompanyPackageD_View>>> GetExpiredCompanyPackages()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();

                    var Results = await Connection.QueryAsync<CompanyPackageD_View>("GetExpiredCompanyPackages", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<CompanyPackageD_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<CompanyPackageD_View>>(false, null);
            }
        }

        public async Task<CompanyPackage> GetWithId(int Id)
        {
            return await MyDbContext.CompanyPackages.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }

        public async Task<Tuple<bool, int>> Save(CompanyPackage_View companyView)
        {
            try
            {

                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SaveCompanyPackage", Connection))
                    {
                        DataTable dt = Utilities.ListExtensions.ToDataTable(companyView.companyPackageDetails);
                        var pList = new SqlParameter("@PackageDetails", SqlDbType.Structured);
                        pList.TypeName = "dbo._CompanyPackageDetails";
                        pList.Value = dt;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@PreRegistrationId", companyView.PreRegistrationId));
                        cmd.Parameters.Add(new SqlParameter("@Logo", companyView.company.Logo));
                        cmd.Parameters.Add(new SqlParameter("@CompanyId", companyView.company.Id));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", companyView.company.Name));
                        cmd.Parameters.Add(new SqlParameter("@CompanyPhoneNumber", companyView.company.PhoneNumber));
                        cmd.Parameters.Add(new SqlParameter("@CompanyEmail", companyView.company.Email));
                        cmd.Parameters.Add(new SqlParameter("@CompanyIsDeleted", companyView.company.IsDeleted));
                        cmd.Parameters.Add(new SqlParameter("@CompanyPackageId", companyView.Id));
                        cmd.Parameters.Add(new SqlParameter("@PackageId", companyView.PackageId));
                        cmd.Parameters.Add(new SqlParameter("@CreationDate", companyView.CreationDate));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", companyView.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", companyView.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@Price", companyView.Price));
                        cmd.Parameters.Add(new SqlParameter("@Currency", companyView.Currency));
                        cmd.Parameters.Add(new SqlParameter("@NumberOfUsers", companyView.NumberOfUsers));
                        cmd.Parameters.Add(new SqlParameter("@IsActive", companyView.IsActive));
                        cmd.Parameters.Add(new SqlParameter("@IsDeleted", companyView.IsDeleted));

                        cmd.Parameters.Add(pList);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            return Tuple.Create<bool, int>(true, reader.GetInt32(0));
                        }

                    }
                    Connection.Close();
                    return Tuple.Create<bool, int>(true, 0);
                }

            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, int>(false, 0);
            }
        }

        public async Task<Tuple<bool, int>> Update(CompanyPackageDetails companyView)
        {
            try
            {

                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    using (SqlCommand cmd = new SqlCommand("UpadateCompanyPackage", Connection))
                    {
                        DataTable dt = Utilities.ListExtensions.ToDataTable(companyView.CPackageDetails);
                        var pList = new SqlParameter("@PackageDetails", SqlDbType.Structured);
                        pList.TypeName = "dbo.CompanyPackageDetails2";
                        pList.Value = dt;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@PreRegistrationId", companyView.PreRegistrationId));
                        cmd.Parameters.Add(new SqlParameter("@CompanyId", companyView.Company.Id));
                        cmd.Parameters.Add(new SqlParameter("@Logo", companyView.Company.Logo));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", companyView.Company.Name));
                        cmd.Parameters.Add(new SqlParameter("@CompanyPhoneNumber", companyView.Company.PhoneNumber));
                        cmd.Parameters.Add(new SqlParameter("@CompanyEmail", companyView.Company.Email));
                        cmd.Parameters.Add(new SqlParameter("@CompanyIsDeleted", companyView.Company.IsDeleted));
                        cmd.Parameters.Add(new SqlParameter("@CompanyPackageId", companyView.Id));
                        cmd.Parameters.Add(new SqlParameter("@PackageId", companyView.PackageId));
                        cmd.Parameters.Add(new SqlParameter("@CreationDate", companyView.CreationDate));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", companyView.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", companyView.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@Price", companyView.Price));
                        cmd.Parameters.Add(new SqlParameter("@Currency", companyView.Currency));
                        cmd.Parameters.Add(new SqlParameter("@NumberOfUsers", companyView.NumberOfUsers));
                        cmd.Parameters.Add(new SqlParameter("@IsActive", companyView.IsActive));
                        cmd.Parameters.Add(new SqlParameter("@IsDeleted", companyView.IsDeleted));

                        cmd.Parameters.Add(pList);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            return Tuple.Create<bool, int>(true, reader.GetInt32(0));
                        }

                    }
                    Connection.Close();
                    return Tuple.Create<bool, int>(true, 0);
                }

            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, int>(false, 0);
            }
        }
    }
}
