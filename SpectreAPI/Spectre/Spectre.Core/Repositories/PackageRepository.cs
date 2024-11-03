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
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger ILogger;
        public PackageRepository(ILogger ILogger, SpectreDBContext context, IConfiguration configuration)
              : base(context)
        {
            this.configuration = configuration;
            this.ILogger = ILogger;

        }

        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, bool>> DeletePackage(int Id)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", Id);

                    var Results = await Connection.QueryAsync<bool>("DeletePackage", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, bool>(true, Results.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, bool>(false, false);
            }
        }

        public async Task<Tuple<bool, List<Package>>> GetAll()
        {
            try
            {
                List<Package> myList = await MyDbContext.Packages.Where(m => m.IsDeleted == false).OrderBy(m => m.Order).ToListAsync();

                return new Tuple<bool, List<Package>>(true, myList);
            }
            catch (Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, List<Package>>(false, null);
            }
        }

        public async Task<Tuple<bool, Package>> GetById(int Id)
        {
            try
            {
                Package myPackage = await MyDbContext.Packages.Where(m => m.Id == Id).SingleOrDefaultAsync();

                return new Tuple<bool, Package>(true, myPackage);
            }
            catch (Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, Package>(false, null);
            }
        }

        public async Task<Tuple<bool, List<PackagePermission_Details>>> GetPackagePagePermissionDetails(int? PackageId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@PackageId", PackageId);

                    var Results = await Connection.QueryAsync<PackagePermission_Details>("GetPackagePagePermissionDetails", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<PackagePermission_Details>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, List<PackagePermission_Details>>(false, null);
            }
        }

        public async Task<Tuple<bool, List<PackagePermission_Extender>>> GetPermissionByPackageId(int PackageId)
        {
            try
            {
                List<PackagePagePermission> permissions = await MyDbContext.PackagePagePermissions
                    .Where(m => (m.PackageId == PackageId && (m.IsDeleted == null || m.IsDeleted == false)))
                    .ToListAsync();

                List<PackagePermission_Extender> list = new List<PackagePermission_Extender>();
                foreach (PackagePagePermission p in permissions)
                {
                    PackagePermission_Extender item = new PackagePermission_Extender();
                    item.Id = p.Id;
                    item.PackageId = p.PackageId;
                    item.PageUrl = p.PageUrl;
                    item.IsDeleted = p.IsDeleted;
                    item.HasCountryLimit = p.HasCountryLimit;

                    list.Add(item);
                }

                return new Tuple<bool, List<PackagePermission_Extender>>(true, list);
            }
            catch (Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, List<PackagePermission_Extender>>(false, null);
            }
        }

        public async Task<IEnumerable<Package>> GetVisiblePackags()
        {
            return await MyDbContext.Packages.Where(m => (m.IsDeleted == false && m.IsVisible == true)).OrderBy(m => m.Order).ToListAsync();
        }

        public async Task<Tuple<bool, ResponseMessage>> SavePackage(Package_View package)
        {
            try
            {

                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SavePackage", Connection))
                    {
                        DataTable dt = Utilities.ListExtensions.ToDataTable(package.PagePermissions);
                        var pList = new SqlParameter("@PackagePermissions", SqlDbType.Structured);
                        pList.TypeName = "dbo.PackagePermissions";
                        pList.Value = dt;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Id", package.Id));
                        cmd.Parameters.Add(new SqlParameter("@NameEn", package.NameEn));
                        cmd.Parameters.Add(new SqlParameter("@NameFr", package.NameFr));
                        cmd.Parameters.Add(new SqlParameter("@NameSpa", package.NameSpa));
                        cmd.Parameters.Add(new SqlParameter("@DescriptionEn", package.DescriptionEn));
                        cmd.Parameters.Add(new SqlParameter("@DescriptionFr", package.DescriptionFr));
                        cmd.Parameters.Add(new SqlParameter("@DescriptionSpa", package.DescriptionSpa));
                        cmd.Parameters.Add(new SqlParameter("@IsDemoPackage", package.IsDemoPackage));
                        cmd.Parameters.Add(new SqlParameter("@IsVisible", package.IsVisible));
                        cmd.Parameters.Add(new SqlParameter("@IsDeleted", package.IsDeleted));
                        cmd.Parameters.Add(new SqlParameter("@Order", package.Order));
                        cmd.Parameters.Add(new SqlParameter("@ImageUrl", package.ImageUrl));
                        if(package.ToYearLimit != 0)
                            cmd.Parameters.Add(new SqlParameter("@ToYear", package.ToYearLimit));
                        if (package.FromYearLimit != 0)
                            cmd.Parameters.Add(new SqlParameter("@FromYear", package.FromYearLimit));
                      

                        cmd.Parameters.Add(pList);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ResponseMessage message = new ResponseMessage();
                            message.Success = reader.GetBoolean(0);
                            message.MessageEn = reader.GetString(1);
                            message.MessageAr = reader.GetString(2);

                            return Tuple.Create<bool, ResponseMessage>(true, message);
                        }

                    }
                    Connection.Close();
                    return Tuple.Create<bool, ResponseMessage>(true, null);
                }

            }
            catch (Exception ex)
            {
                await ILogger.LogException(ex);
                return new Tuple<bool, ResponseMessage>(false, null);
            }
        }
    }
}
