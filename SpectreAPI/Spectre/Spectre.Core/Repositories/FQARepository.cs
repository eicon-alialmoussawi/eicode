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
    public class FQARepository : Repository<Fqa>, IFQARepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public FQARepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
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


                    var Results = await Connection.ExecuteAsync("DeleteFQA", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<IEnumerable<Fqa>> GetAll()
        {
            return await MyDbContext.Fqas.Where(m => m.IsDeleted == false).ToListAsync();
        }

        public async Task<Tuple<bool>> Save(Fqa Fqa)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Fqa.Id);
                    Params.Add("@QuestionEn", Fqa.QuestionEn);
                    Params.Add("@QuestionAr", Fqa.QuestionAr);
                    Params.Add("@QuestionFr", Fqa.QuestionFr);
                    Params.Add("@AnswerEn", Fqa.AnswerEn);
                    Params.Add("@AnswerAr", Fqa.AnswerAr);
                    Params.Add("@AnswerFr", Fqa.AnswerFr);
                    Params.Add("@IsDeleted", Fqa.IsDeleted);


                    var Results = await Connection.ExecuteAsync("SaveFQA", Params, commandType: CommandType.StoredProcedure);
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
