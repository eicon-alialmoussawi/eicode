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

namespace Spectre.Core.Repositories
{
    public class PermissionRepository : Repository<AspnetPermission>, IPermissionRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger Logger;
        public PermissionRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }
        public async Task<Tuple<bool, List<AspnetPermission>>> GetUserPermissions(string IdentityUserId, string Action, string PageUrl)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", new Guid(IdentityUserId));
                    Params.Add("@Action", (Action ?? ""));
                    Params.Add("@PageUrl", (PageUrl ?? ""));
                    var Results = await Connection.QueryAsync<AspnetPermission>("GetUserPermissions", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<AspnetPermission>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<AspnetPermission>>(false, null);
            }
        }

        public async Task<Tuple<bool, List<AspnetPermission>>> GetAllPermissions()
        {
            try
            {
                return new Tuple<bool, List<AspnetPermission>>(true, MyDbContext.AspnetPermissions.OrderBy(m=> m.MenuOrder).ToList());
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<AspnetPermission>>(false, null);
            }
        }

        public async Task<Tuple<bool, bool>> CheckIfUserCanView(int UserId, string PageUrl)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);
                    Params.Add("@PageUrl", PageUrl);
                    var Results = await Connection.QueryAsync<bool>("CheckIfUserCanView", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, bool>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }
    }
}
