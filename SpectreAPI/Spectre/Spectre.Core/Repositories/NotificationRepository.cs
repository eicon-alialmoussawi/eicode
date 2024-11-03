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
    public class NotificationRepository :  Repository<UserNotification>, INotificationRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public NotificationRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, NotifcationMessage_View>> GetUnSeenNotifications(int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);

                    var Results = await Connection.QueryAsync<NotifcationMessage_View>("GetUnSeenNotifications", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, NotifcationMessage_View>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, NotifcationMessage_View>(false, null);
            }
        }

        public async Task<Tuple<bool, List<UserNotification_View>>> GetUserNotifications(int UserId, string Status)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);
                    Params.Add("@Status", Status);

                    var Results = await Connection.QueryAsync<UserNotification_View>("GetUserNotifications", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<UserNotification_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<UserNotification_View>>(false, null);
            }
        }

        public async Task<Tuple<bool>> SendNotification(string TextEn, string TextAr, string TextFr)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@TextEn", TextEn);
                    Params.Add("@TextAr", TextAr);
                    Params.Add("@TextFr", TextFr);

                    var Results = await Connection.ExecuteAsync("SendNotification", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<Tuple<bool>> SetNotificationAsViewed(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.ExecuteAsync("SetNotificationAsViewed", Params, commandType: CommandType.StoredProcedure);
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

        public async Task<Tuple<bool>> SetUserNotificationAsSeen(int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@UserId", UserId);

                    var Results = await Connection.ExecuteAsync("SetUserNotificationAsSeen", Params, commandType: CommandType.StoredProcedure);
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
