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
    public class ContactUsRepository : Repository<ContactU>, IContactUsRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public ContactUsRepository(IConfiguration configuration, SpectreDBContext context, ILogger Logger)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, ContactU>> Create(ContactU contact)
        {
            try
            {
                var Result = await MyDbContext.ContactUs.AddAsync(contact);
                MyDbContext.SaveChanges();

                return new Tuple<bool, ContactU>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, ContactU>(false, null);
            }
        }

        public async Task<Tuple<bool>> DeleteContact(int Id)
        {
            try
            {
                var _contact = await MyDbContext.ContactUs.Where(m => m.Id == Id).SingleOrDefaultAsync();
                _contact.IsDeleted = true;
                var Result = MyDbContext.ContactUs.Update(_contact);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(_contact).State = EntityState.Detached;

                return new Tuple<bool>(true);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool>(false);
            }
        }

        public async Task<Tuple<bool, List<ContactUs_View>>> GetAlll()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();

                    var Results = await Connection.QueryAsync<ContactUs_View>("GetContactUs", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<ContactUs_View>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<ContactUs_View>>(false, null);
            }
        }
    }
}
