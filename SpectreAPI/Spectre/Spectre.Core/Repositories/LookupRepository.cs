using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class LookupRepository : Repository<Lookup>, ILookupRepository
    {
        private readonly IConfiguration configuration;
        public LookupRepository(IConfiguration configuration, SpectreDBContext context)
            : base(context)
        {
            this.configuration = configuration;
        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }
        public async Task<IEnumerable<Lookup>> GetLookupsByParantCode(string code)
        {
            var parentLookup = await MyDbContext.Lookups.SingleOrDefaultAsync(m => m.LookupCode == code);
            return await MyDbContext.Lookups.Where(m => m.ParentId == parentLookup.Id).OrderBy(m => m.Name.ToUpper()).ToListAsync();
        }

        public async Task<Tuple<bool, Lookup>> Create(Lookup Lookup)
        {

            try
            {
                var Result = await MyDbContext.Lookups.AddAsync(Lookup);
                MyDbContext.SaveChanges();

                return new Tuple<bool, Lookup>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, Lookup>(false, null);
            }
        }

        public async Task<Tuple<bool, Lookup>> Update(Lookup Lookup)
        {

            try
            {
                var Result = MyDbContext.Lookups.Update(Lookup);

                MyDbContext.SaveChanges();

                MyDbContext.Entry(Lookup).State = EntityState.Detached;
                return new Tuple<bool, Lookup>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, Lookup>(false, null);
            }
        }
        public async Task<IEnumerable<Lookup>> GetAll()
        {
            return await MyDbContext.Lookups.ToListAsync();
        }
        public async Task<Lookup> GetLookupByCode(string code)
        {
            return await MyDbContext.Lookups.Where(m => m.LookupCode == code).SingleOrDefaultAsync();
        }

        public async Task<Lookup> GetLookupById(int id)
        {
            return await MyDbContext.Lookups.Where(m => m.Id == id).SingleOrDefaultAsync();
        }
        public async Task<Lookup> GetById(int id)
        {
            return await MyDbContext.Lookups.Where(m => m.Id == id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<Lookup> GetByName(string Name)
        {
            return await MyDbContext.Lookups.Where(m => m.Name == Name).SingleOrDefaultAsync();
        }

        public async Task<Tuple<bool, List<Lookup>>> GetLookupsBySocialCode(bool IsIMF)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@IsIMF", IsIMF);


                    var Results = await Connection.QueryAsync<Lookup>("GetLookupsBySocialCode", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<Lookup>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, List<Lookup>>(true, null);
            }
        }
    }
}
