using Dapper;
using Microsoft.EntityFrameworkCore;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Spectre.Core.RepositoryHandler;
using System.Data;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Repositories
{
    public class RoleRepository: Repository<RolePermission>, IRoleRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger Logger;
        public RoleRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        public async Task<Tuple<bool, bool>> SaveRolePermissions(Guid RoleId, string Permissions)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    SqlCommand Command = new SqlCommand("SaveRolePermissions", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@RoleId", RoleId);
                    Command.Parameters.AddWithValue("@Permissions", Permissions);
                    Command.ExecuteNonQuery();
                    Connection.Close();
                    return new Tuple<bool, bool>(true, true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }

        public async Task<Tuple<bool, bool>> DeleteRolePermissions(string RoleId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    SqlCommand Command = new SqlCommand("DeleteRolePermissions", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@RoleId", RoleId);
                    Command.ExecuteNonQuery();
                    Connection.Close();
                    return new Tuple<bool, bool>(true, true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }

        public async Task<Tuple<bool, RoleResponse>> GetRoleById(Guid RoleId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@RoleId", RoleId);

                    var Results = await Connection.QueryAsync<RoleResponse>("GetRoleById", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, RoleResponse>(true, Results.ToList().FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, RoleResponse>(false, null);
            }
        }
    }
}
