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
    public class SystemSettingsRepository
        : Repository<SystemSetting>, ISystemSettingsRepository
    {
        private readonly ILogger Logger;
        public SystemSettingsRepository(SpectreDBContext context, ILogger Logger)
            : base(context)
        {
            this.Logger = Logger;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }
        public async Task<Tuple<bool, SystemSetting>> Create(SystemSetting SystemSetting)
        {
            try
            {
                var Result = await MyDbContext.SystemSettings.AddAsync(SystemSetting);
                MyDbContext.SaveChanges();

                return new Tuple<bool, SystemSetting>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, SystemSetting>(false, null);
            }
        }

        public async Task<IEnumerable<SystemSetting>> GetAll()
        {
            return await MyDbContext.SystemSettings.ToListAsync();
        }

        public async Task<SystemSetting> GetById(int Id)
        {
            return await MyDbContext.SystemSettings.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }

        public async Task<Tuple<bool, SystemSetting>> Update(SystemSetting SystemSetting)
        {
            try
            {
                var Result = MyDbContext.SystemSettings.Update(SystemSetting);
                MyDbContext.SaveChanges();

                return new Tuple<bool, SystemSetting>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, SystemSetting>(false, null);
            }
        }
    }
}
