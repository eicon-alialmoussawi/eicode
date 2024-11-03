using Microsoft.EntityFrameworkCore;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Spectre.Core.Repositories
{
 public   class LatestNewsRepository
     : Repository<LatestNews>, ILatestNewsRepository
    {
        private readonly ILogger Logger;
        public LatestNewsRepository(SpectreDBContext context, ILogger Logger)
            : base(context)
        {
            this.Logger = Logger;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, LatestNews>> Create(LatestNews LatestNews)
        {
            try
            {
                var Result = await MyDbContext.LatestNews.AddAsync(LatestNews);
                MyDbContext.SaveChanges();

                return new Tuple<bool, LatestNews>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, LatestNews>(false, null);
            }
        }

        public async Task<IEnumerable<LatestNews>> GetAll()
        {
            return await MyDbContext.LatestNews.Where(m => m.IsDeleted == false).ToListAsync();
        }

        public async Task<LatestNews> GetById(int Id)
        {
            return await MyDbContext.LatestNews.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<LatestNews>> GetPublishedNews()
        {
            return await MyDbContext.LatestNews.Where(m => (m.IsDeleted == false && m.IsPublished == true)).ToListAsync();
        }

        public async Task<Tuple<bool, LatestNews>> Update(LatestNews LatestNews)
        {
            try
            {
                var Result = MyDbContext.LatestNews.Update(LatestNews);
                MyDbContext.SaveChanges();

                return new Tuple<bool, LatestNews>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, LatestNews>(false, null);
            }
        }
    }
}
