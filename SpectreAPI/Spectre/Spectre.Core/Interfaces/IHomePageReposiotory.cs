using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spectre.Core.Interfaces
{
    public interface IHomePageReposiotory
    {
        Task<IEnumerable<AboutSpectre>> GetAbout();
        Task<Tuple<bool, AboutSpectre>> UpdateAbout(AboutSpectre about);
        Task<IEnumerable<Service>> GetServices();
        Task<Tuple<bool, Service>> UpdateService(Service service);
        Task<IEnumerable<VisualizingReport>> GetVisualize();
        Task<Tuple<bool, VisualizingReport>> UpdateVisualize(VisualizingReport report);
        Task<IEnumerable<HomePageHeader>> GetWelcome();
        Task<Tuple<bool, HomePageHeader>> UpdateWelcome(HomePageHeader welcome);
        Task<IEnumerable<ReportSnap>> GetReportSnaps();
        Task<Tuple<bool, ReportSnap>> UpdateSnap(ReportSnap snap);
        Task<Tuple<bool, ReportSnap>> DeleteSnap(int Id);
        Task<Tuple<bool, ReportSnap>> Save(ReportSnap snap);
        Task<IEnumerable<SpectreFooter>> GetFooter();
        Task<Tuple<bool, SpectreFooter>> UpdateFooter(SpectreFooter SpectreFooter);

        Task<IEnumerable<HelpAboutU>> GetHelpAbout();
        Task<Tuple<bool, HelpAboutU>> UpdateHelpAbout(HelpAboutU about);
    }
}
