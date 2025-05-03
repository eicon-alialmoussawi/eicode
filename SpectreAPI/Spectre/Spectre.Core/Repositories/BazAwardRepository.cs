using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class BazAwardRepository : Repository<BazAward>, IBazAwardRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public BazAwardRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
          : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }
        public async Task<BazAward> GetById(int Id)
        {
            return await MyDbContext.BazAwards.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<BazAward>> GetAll()
        {
            return await MyDbContext.BazAwards.ToListAsync();
        }
        public async Task<IEnumerable<BazAwardView>> GetAllForView()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    var Results = await Connection.QueryAsync<BazAwardView>("GetAllBazAwards", Params, commandType: CommandType.StoredProcedure);
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
        public async Task<Tuple<bool, List<CountyAwardData>>> GetCountyAwardsCountAsync(BazAwardFilter_View operatorIdsRequest = null)
        {
            try
            {
                var results = new List<CountyAwardData>();

                // Convert the list of operator IDs to a comma-separated string if provided
                string operatorIdParam = null;
                if (operatorIdsRequest?.Licenses != null && operatorIdsRequest.Licenses.Count > 0)
                {
                    operatorIdParam = string.Join(",", operatorIdsRequest.Licenses);
                }

                using (var connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetCountyAwardsCount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@OperatorIds", (object)operatorIdParam ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new CountyAwardData
                                {
                                    CountyName = reader["county_name"].ToString(),
                                    StateName = reader["state_name"].ToString(),
                                    FipsCode = reader["fips_code"].ToString(),
                                    AwardsCount = Convert.ToInt32(reader["AwardsCount"])
                                });
                            }
                        }
                    }
                }

                return new Tuple<bool, List<CountyAwardData>>(true, results);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<CountyAwardData>>(false, null);
            }
        }



        public async Task<Tuple<bool, List<BazAwardView>>> GetFilteredBazAwards(BazAwardFilter_View view)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@BandIds", !view.BandIds.IsNullOrEmpty() ? string.Join(",", view.BandIds) : null);
                    Params.Add("@MarketCodesId", !view.MarketCodeIds.IsNullOrEmpty() ? string.Join(",", view.MarketCodeIds) : null);
                    Params.Add("@AuctionIds", !view.AuctionIds.IsNullOrEmpty() ? string.Join(",", view.AuctionIds) : null);
                    Params.Add("@LicenseIds", !view.Licenses.IsNullOrEmpty() ? string.Join(",", view.Licenses) : null);
                    Params.Add("@MarketClassIds", !view.Licenses.IsNullOrEmpty() ? string.Join(",", view.MarketClassIds) : null);

                    var results = await Connection.QueryAsync<BazAwardView>(
                        "GetFilteredBazAwards",
                        Params,
                        commandType: CommandType.StoredProcedure
                    );
                    Connection.Close();
                    return new Tuple<bool, List<BazAwardView>>(true, results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<BazAwardView>>(false, null);
            }


        }
        public async Task<Tuple<bool, List<BazAwardViewForPricing>>> GetFilteredBazAwardsForPricing(BazAwardFilter_ViewForPricing view)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();

                    Params.Add("@BandIds", !view.BandIds.IsNullOrEmpty() ? string.Join(",", view.BandIds) : null);
                    Params.Add("@MarketCodesId", !view.MarketCodeIds.IsNullOrEmpty() ? string.Join(",", view.MarketCodeIds) : null);
                    Params.Add("@AuctionIds", !view.AuctionIds.IsNullOrEmpty() ? string.Join(",", view.AuctionIds) : null);
                    Params.Add("@LicenseIds", !view.Licenses.IsNullOrEmpty() ? string.Join(",", view.Licenses) : null);
                    Params.Add("@MarketClassIds", !view.MarketClassIds.IsNullOrEmpty() ? string.Join(",", view.MarketClassIds) : null);

                    Params.Add("@MinGDP", view.MinGDPc);
                    Params.Add("@MaxGDP", view.MaxGDPc);
                    Params.Add("@MinPopulation", view.MinPopulation);
                    Params.Add("@MaxPopulation", view.MaxPopulation);

                    Params.Add("@AdjustForInflation", view.AdjustForInflation == null ? false : view.AdjustForInflation); // should be boolean
                    Params.Add("@IssueDate", view.IssueDate); // should be int year, like 2023

                    var results = await Connection.QueryAsync<BazAwardViewForPricing>(
                        "GetFilteredBazAwardsForPricing", // updated SP name
                        Params,
                        commandType: CommandType.StoredProcedure
                    );

                    return new Tuple<bool, List<BazAwardViewForPricing>>(true, results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<BazAwardViewForPricing>>(false, null);
            }
        }

    }
}
