using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Spectre.Core.RepositoryHandler;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger Logger;
        public UserRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, User>> GetByEmail(string Email)
        {
            try
            {
                var User = await MyDbContext.Users.FirstOrDefaultAsync(c => c.Email == Email && c.IsDeleted == false);
                return Tuple.Create<bool, User>(true, User);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, User>(false, null);
            }
        }
        public async Task<Tuple<bool, User>> Create(User User)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    SqlCommand Command = new SqlCommand("CreateUser", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@Id", User.Id);
                    Command.Parameters.AddWithValue("@Name", User.Name);
                    Command.Parameters.AddWithValue("@UserName", User.UserName);
                    Command.Parameters.AddWithValue("@Email", User.Email);
                    Command.Parameters.AddWithValue("@PhoneNumber", User.PhoneNumber);
                    Command.Parameters.AddWithValue("@IdentityUserId", User.IdentityUserId);
                    Command.Parameters.AddWithValue("@IsAdmin", User.IsAdmin);
                    Command.Parameters.AddWithValue("@IsApproved", User.IsApproved);
                    Command.Parameters.AddWithValue("@IsLocked", User.IsLocked);
                    Command.Parameters.AddWithValue("@LastName", User.LastName);
                    Command.Parameters.AddWithValue("@IsDeleted", User.IsDeleted);
                    Command.Parameters.AddWithValue("@UserType", User.UserType);
                    Command.Parameters.AddWithValue("@CompanyId", User.CompanyId);
                    Command.Parameters.AddWithValue("@JobTitle", User.JobTitle);
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }
                return Tuple.Create<bool, User>(true, User);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, User>(false, null);
            }
        }
        public async Task<Tuple<bool, List<User>>> GetAll()
        {
            try
            {
                var Results = await MyDbContext.Users.AsNoTracking().Where(x => x.IsDeleted == null || x.IsDeleted == false).ToListAsync();

                return Tuple.Create<bool, List<User>>(true, Results.ToList());
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, List<User>>(false, null);
            }
        }
        public async Task<Tuple<bool, List<User>>> GetAllExistingUsers()
        {
            try
            {
                var Results = await MyDbContext.Users.Where(x => x.IsDeleted == false && x.UserType == "U_SYS").AsNoTracking().ToListAsync();
                return Tuple.Create<bool, List<User>>(true, Results.ToList());
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, List<User>>(false, null);
            }
        }
        public async Task<Tuple<bool, User>> GetByIdentityId(string IdentityId)
        {

            try
            {
                var User = await MyDbContext.Users.FirstOrDefaultAsync(c => c.IdentityUserId == IdentityId);
                return Tuple.Create<bool, User>(true, User);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, User>(false, null);
            }
        }
        public async Task<Tuple<bool, User>> GetById(int UserId)
        {

            try
            {
                var User = await MyDbContext.Users.FirstOrDefaultAsync(c => c.Id == UserId && c.IsDeleted == false);
                return Tuple.Create<bool, User>(true, User);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, User>(false, null);
            }
        }
        public async Task<Tuple<bool, User>> Update(User User)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    SqlCommand Command = new SqlCommand("CreateUser", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@Id", User.Id);
                    Command.Parameters.AddWithValue("@UserName", User.UserName);
                    Command.Parameters.AddWithValue("@Email", User.Email);
                    Command.Parameters.AddWithValue("@Name", User.Name);
                    Command.Parameters.AddWithValue("@PhoneNumber", User.PhoneNumber);
                    Command.Parameters.AddWithValue("@IdentityUserId", User.IdentityUserId);
                    Command.Parameters.AddWithValue("@IsAdmin", User.IsAdmin);
                    Command.Parameters.AddWithValue("@IsApproved", User.IsApproved);
                    Command.Parameters.AddWithValue("@IsLocked", User.IsLocked);
                    Command.Parameters.AddWithValue("@LastName", User.LastName);
                    Command.Parameters.AddWithValue("@IsDeleted", User.IsDeleted);
                    Command.Parameters.AddWithValue("@DOB", User.Dob);
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }
                return Tuple.Create<bool, User>(true, User);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, User>(false, null);
            }
        }
        public async Task<Tuple<bool, string>> GenerateRandomUserName()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    var Results = await Connection.QueryAsync<string>("GenerateRandomUserName", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, string>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, string>(false, "");
            }
        }
        public async Task<Tuple<bool, List<Page>>> GetClientMenu(int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);

                    var Results = await Connection.QueryAsync<Page>("GetClientMenu", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<Page>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<Page>>(false, null);
            }
        }
        public async Task<Tuple<bool, List<User_View2>>> GetUsersByCompanyId(int? CompanyId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@CompanyId", CompanyId);
                    var Results = await Connection.QueryAsync<User_View2>("GetUsersByCompanyId", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<User_View2>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<User_View2>>(false, null);
            }
        }
        public async Task<Tuple<bool>> DeleteCompanyUsers(string Ids, int CompanyId, bool? IsActive)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Ids", Ids);
                    Params.Add("@CompanyId", CompanyId);
                    Params.Add("@IsActive", IsActive);
                    var Results = await Connection.ExecuteAsync("DeleteCompanyUsers", Params, commandType: CommandType.StoredProcedure);
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
        public async Task<Tuple<bool>> DeleteUser(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.ExecuteAsync("DeleteUser", Params, commandType: CommandType.StoredProcedure);
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
        public async Task<Tuple<bool>> UpdateUserLogInStatus(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.ExecuteAsync("UpdateUserLogInStatus", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<Tuple<bool, UserInfo_View>> GetUserInfo(int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);
                    var Results = await Connection.QueryAsync<UserInfo_View>("GetUserInfo", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, UserInfo_View>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, UserInfo_View>(false, null);
            }
        }

        public async Task<Tuple<bool, List<UserFeatures_View>>> GetUserFeaturs(int UserId, string Lang)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);
                    Params.Add("@Lang", Lang);

                    var Results = await Connection.QueryAsync<UserFeatures_View>("GetUserFeaturs", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<UserFeatures_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<UserFeatures_View>>(false, null);
            }
        }
    }
}
