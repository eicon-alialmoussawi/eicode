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
        public async Task<Tuple<bool, List<BazAwardView>>> GetFilteredBazAwards(BazAwardFilter_View view)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@RegionIds", view.Regions != null ? string.Join(",", view.Regions) : null);
                    Params.Add("@StatesId", view.States != null ? string.Join(",", view.States) : null);
                    Params.Add("@CountiesId", view.Counties != null ? string.Join(",", view.Counties) : null);
                    Params.Add("@AuctionIds", view.AuctionIds != null ? string.Join(",", view.AuctionIds) : null);

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
    }
}
