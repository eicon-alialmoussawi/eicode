using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class OperatorRepository : Repository<Operator>, IOperatorRepository
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public OperatorRepository(IConfiguration configuration, SpectreDBContext context, ILogger logger)
            : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Create a new operator
        public async Task<Tuple<bool, Operator>> Create(Operator operatorEntity)
        {
            try
            {
                var result = await MyDbContext.Operators.AddAsync(operatorEntity);
                await MyDbContext.SaveChangesAsync();
                return new Tuple<bool, Operator>(true, result.Entity);
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return Tuple.Create<bool, Operator>(false, null);
            }
        }

        // Get all operators
        public async Task<IEnumerable<Operator>> GetAll()
        {
            return await MyDbContext.Operators.ToListAsync();
        }

        // Get operator by ID
        public async Task<Operator> GetById(int id)
        {
            return await MyDbContext.Operators
                                     .Where(o => o.OperatorID == id)
                                     .SingleOrDefaultAsync();
        }



        // Example of a custom stored procedure or advanced query using Dapper (for user-specific operators)
        public async Task<Tuple<bool, List<Operator>>> GetUserOperators(int userId, string pageUrl, string source, string lang)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId);
                    parameters.Add("@PageUrl", pageUrl);
                    parameters.Add("@Source", source);
                    parameters.Add("@Lang", lang);

                    var results = await connection.QueryAsync<Operator>("GetUserOperators", parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return new Tuple<bool, List<Operator>>(true, results.ToList());
                }
            }
            catch (Exception ex)
            {
                await _logger.LogException(ex);
                return new Tuple<bool, List<Operator>>(false, null);
            }
        }



        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }
    }
}
