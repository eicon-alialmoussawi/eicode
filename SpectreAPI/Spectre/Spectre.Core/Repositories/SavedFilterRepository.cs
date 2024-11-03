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
    public class SavedFilterRepository : ISavedFilterRepository
    {

        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public SavedFilterRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }

        public async Task<Tuple<bool, List<SavedFilters_View>>> GetUsersSavedFilters(string PageUrl, int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@PageUrl", PageUrl);
                    Params.Add("@UserId", UserId);

                    var Results = await Connection.QueryAsync<SavedFilters_View>("GetUsersSavedFilters", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<SavedFilters_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<SavedFilters_View>>(false, null);
            }
        }

        public async Task<Tuple<bool, List<SavedFilters_View>>> GetUserDefaultSettings(int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);

                    var Results = await Connection.QueryAsync<SavedFilters_View>("GetUserDefaultSettings", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<SavedFilters_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<SavedFilters_View>>(false, null);
            }
        }

        public async Task<Tuple<bool, bool>> SaveUserFilters(List<SavedFilters_View> view)
        {
            try
            {

                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SaveUserFilters", Connection))
                    {
                        DataTable dt = Utilities.ListExtensions.ToDataTable(view);
                        var pList = new SqlParameter("@Filters", SqlDbType.Structured);
                        pList.TypeName = "dbo.FiltersValues";
                        pList.Value = dt;

                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.Add(pList);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            return Tuple.Create<bool, bool>(true, reader.GetBoolean(0));
                        }

                    }
                    Connection.Close();
                }
                return new Tuple<bool, bool>(false, false);

            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }
    }
}
