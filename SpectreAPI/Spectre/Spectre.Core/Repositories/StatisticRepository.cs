using System;
using System.Collections.Generic;
using System.Data;
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
    public class StatisticRepository : IStatisticRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public StatisticRepository(IConfiguration configuration, ILogger Logger)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }

        public async Task<Tuple<bool, ActivePackage_View>> ActivePackagesStatistics()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.QueryAsync<ActivePackage_View>("ActivePackagesStatistics", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, ActivePackage_View>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, ActivePackage_View>(false, null);
            }
        }

        public async Task<Tuple<bool, Statistic_View>> GetStatistics()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.QueryAsync<Statistic_View>("GetStatistics", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, Statistic_View>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, Statistic_View>(false, null);
            }
        }

        public async Task<Tuple<bool, List<CustomerPackage_View>>> GetTotalCustomersPerPackage()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.QueryAsync<CustomerPackage_View>("GetTotalCustomersPerPackage", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<CustomerPackage_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<CustomerPackage_View>>(false, null);
            }
        }

        public async Task<Tuple<bool, List<Sales_View>>> GetTotalSales()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.QueryAsync<Sales_View>("GetTotalSales", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<Sales_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<Sales_View>>(false, null); 
            }
        }
    }
}
