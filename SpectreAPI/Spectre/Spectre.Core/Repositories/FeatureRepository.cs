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
    public class FeatureRepository : IFeatureRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;

        public FeatureRepository(IConfiguration configuration, SpectreDBContext context, ILogger Logger)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }

        public async Task<Tuple<bool>> Delete(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.ExecuteAsync("DeleteFeature", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<Tuple<bool, List<Feature>>> GetAll()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();

                    var Results = await Connection.QueryAsync<Feature>("GetAllFeatures", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<Feature>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<Feature>>(false, null);
            }
        }

        public async Task<Tuple<bool>> Save(Feature Feature)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Feature.Id);
                    Params.Add("@TitleEn", Feature.TitleEn);
                    Params.Add("@TitleAr", Feature.TitleAr);
                    Params.Add("@TitleFr", Feature.TitleFr);
                    Params.Add("@DescriptionEn", Feature.DescriptionEn);
                    Params.Add("@DescriptionAr", Feature.DescriptionAr);
                    Params.Add("@DescriptionFr", Feature.DescriptionFr);
                    Params.Add("@ImageUrl", Feature.ImageUrl);
                    Params.Add("@IsDeleted", Feature.IsDeleted);


                    var Results = await Connection.ExecuteAsync("SaveFeatures", Params, commandType: CommandType.StoredProcedure);
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
