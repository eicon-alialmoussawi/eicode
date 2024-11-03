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
    public class HomePageReposiotory : Repository<AboutSpectre>, IHomePageReposiotory
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public HomePageReposiotory(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, ReportSnap>> DeleteSnap(int Id)
        {
            try
            {
                var snap = await MyDbContext.ReportSnaps.Where(m => m.Id == Id).SingleOrDefaultAsync();
                snap.IsDeleted = true;
                var Result = MyDbContext.ReportSnaps.Update(snap);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(snap).State = EntityState.Detached;

                return new Tuple<bool, ReportSnap>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, ReportSnap>(false, null);
            }
        }

        public async Task<IEnumerable<AboutSpectre>> GetAbout()
        {
            return await MyDbContext.AboutSpectres.ToListAsync();
        }

        public async Task<IEnumerable<SpectreFooter>> GetFooter()
        {
            return await MyDbContext.SpectreFooters.ToListAsync();
        }

        public async Task<IEnumerable<HelpAboutU>> GetHelpAbout()
        {
            return await MyDbContext.HelpAboutUs.ToListAsync();
        }

        public async Task<IEnumerable<ReportSnap>> GetReportSnaps()
        {
            return await MyDbContext.ReportSnaps.Where(m => m.IsDeleted == null || m.IsDeleted == false ).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetServices()
        {
            return await MyDbContext.Services.ToListAsync();
        }

        public async Task<IEnumerable<VisualizingReport>> GetVisualize()
        {
            return await MyDbContext.VisualizingReports.ToListAsync();
        }

        public async Task<IEnumerable<HomePageHeader>> GetWelcome()
        {
            return await MyDbContext.HomePageHeaders.ToListAsync();
        }

        public async Task<Tuple<bool, ReportSnap>> Save(ReportSnap snap)
        {
            try
            {

                var Result = MyDbContext.ReportSnaps.Add(snap);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(snap).State = EntityState.Detached;

                return new Tuple<bool, ReportSnap>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, ReportSnap>(false, null);
            }
        }

        public async Task<Tuple<bool, AboutSpectre>> UpdateAbout(AboutSpectre about)
        {
            try
            {

                var Result = MyDbContext.AboutSpectres.Update(about);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(about).State = EntityState.Detached;

                return new Tuple<bool, AboutSpectre>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, AboutSpectre>(false, null);
            }
        }

        public async  Task<Tuple<bool, SpectreFooter>> UpdateFooter(SpectreFooter SpectreFooter)
        {
            try
            {
                var Result = MyDbContext.SpectreFooters.Update(SpectreFooter);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(SpectreFooter).State = EntityState.Detached;

                return new Tuple<bool, SpectreFooter>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, SpectreFooter>(false, null);
            }
        }

        public async Task<Tuple<bool, HelpAboutU>> UpdateHelpAbout(HelpAboutU about)
        {
            try
            {

                var Result = MyDbContext.HelpAboutUs.Update(about);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(about).State = EntityState.Detached;

                return new Tuple<bool, HelpAboutU>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, HelpAboutU>(false, null);
            }
        }

        public async Task<Tuple<bool, Service>> UpdateService(Service service)
        {
            try
            {

                var Result = MyDbContext.Services.Update(service);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(service).State = EntityState.Detached;

                return new Tuple<bool, Service>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, Service>(false, null);
            }
        }

        public async Task<Tuple<bool, ReportSnap>> UpdateSnap(ReportSnap snap)
        {
            try
            {

                var Result = MyDbContext.ReportSnaps.Update(snap);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(snap).State = EntityState.Detached;

                return new Tuple<bool, ReportSnap>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, ReportSnap>(false, null);
            }
        }

        public async Task<Tuple<bool, VisualizingReport>> UpdateVisualize(VisualizingReport report)
        {
            try
            {

                var Result = MyDbContext.VisualizingReports.Update(report);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(report).State = EntityState.Detached;

                return new Tuple<bool, VisualizingReport>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, VisualizingReport>(false, null);
            }
        }

        public async Task<Tuple<bool, HomePageHeader>> UpdateWelcome(HomePageHeader welcome)
        {
            try
            {

                var Result = MyDbContext.HomePageHeaders.Update(welcome);
                MyDbContext.SaveChanges();
                MyDbContext.Entry(welcome).State = EntityState.Detached;

                return new Tuple<bool, HomePageHeader>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, HomePageHeader>(false, null);
            }
        }
    }
}
