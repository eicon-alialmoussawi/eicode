using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.RepositoryHandler
{

    public class Logger : ILogger
    {
        private readonly SpectreDBContext dbContext;
        private readonly IConfiguration Config;
        public Logger(SpectreDBContext dbContext, IConfiguration Config)
        {
            this.dbContext = dbContext;
            this.Config = Config;
        }
        public async Task LogException(Exception ex)
        {
            ExceptionLog Log = new ExceptionLog();
            Log.Id = 0;
            Log.Subject = ex.Source + " - " + ex.StackTrace;
            Log.Details = ex.Message;
            Log.InnerMessage = ex.InnerException.Message;
            Log.ExceptionDate = DateTime.Now;
            dbContext.ExceptionLogs.Add(Log);
            await dbContext.SaveChangesAsync();
        }

        public async Task LogCustomException(string Source, string Message)
        {
            ExceptionLog Log = new ExceptionLog();
            Log.Id = 0;
            Log.Subject = Source;
            Log.Details = Message;
            Log.InnerMessage = "";
            Log.ExceptionDate = DateTime.Now;
            dbContext.ExceptionLogs.Add(Log);
            await dbContext.SaveChangesAsync();
        }

        public async Task LogUserAction(UserActionLog Log)
        {
            try
            {
                dbContext.UserActionLogs.Add(Log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await this.LogException(ex);
            }
        }

        public async Task<Tuple<bool, List<Models.Extenders.ActionLogs>>> GetActionLogs(int Action, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(Config.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Action", Action);
                    Params.Add("@FromDate", FromDate);
                    Params.Add("@ToDate", ToDate);
                    var Results = await Connection.QueryAsync<Models.Extenders.ActionLogs>("GetActionLogs", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<Models.Extenders.ActionLogs>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await this.LogException(ex);
                return new Tuple<bool, List<Models.Extenders.ActionLogs>>(false, null);
            }
        }

        public async Task<IEnumerable<ExceptionLog>> GetExceptionLogs()
        {
            return await dbContext.ExceptionLogs.OrderByDescending(m => m.ExceptionDate).ToListAsync();
        }

        public async Task<Tuple<bool, List<LoginDetails>>> GetLoginDetails(DateTime date, int userId)
        {
            try
            {
                List<LoginDetails> loginDetailsList = new List<LoginDetails>();
                using (SqlConnection Connection = new SqlConnection(Config.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    var MinDate = DateTime.ParseExact("01/01/1980", "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    Connection.Open();
                    DataTable dbResult = new DataTable();
                    SqlCommand command = Connection.CreateCommand();
                    command.CommandText = "GetLoginDetails";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Date", date < MinDate ? null : date);
                    command.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dbResult);

                    if (dbResult != null && dbResult.Rows.Count > 0)
                    {
                        foreach (DataRow row in dbResult.Rows)
                        {
                            LoginDetails loginDetails = new LoginDetails();
                            loginDetails.UserId  = row.IsNull("UserId") ? 0 : row.Field<int>("UserId");
                            loginDetails.UserName = row.IsNull("UserName") ? "" : row.Field<string>("UserName");
                            loginDetails.IPAddress = row.IsNull("IPAddress") ? "" : row.Field<string>("IPAddress");
                            loginDetails.LoginDate = row.IsNull("LoginDate") ? DateTime.MinValue : row.Field<DateTime>("LoginDate");

                            loginDetailsList.Add(loginDetails);
                        }


                    }

                    Connection.Close();
                    return new Tuple<bool, List<LoginDetails>>(true, loginDetailsList);
                }
            }
            catch (Exception ex)
            {
                await this.LogException(ex);
                return new Tuple<bool, List<LoginDetails>>(false, null);
            }
        }
    }
}
