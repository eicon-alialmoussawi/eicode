using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class HomePageContet_View
    {
        public List<AboutSpectre> AboutUs { get; set; }
        public List<Service> Services { get; set; }
        public List<VisualizingReport> Visualizing { get; set; }
        public List<HomePageHeader> HomePageHeader { get; set; }
        public List<ReportSnap> ReportSnaps { get; set; }
        public List<Package> Packages { get; set; }
        public List<LatestNews> LatestNews { get; set; }
        public List<Country> Countries { get; set; }
        public List<SpectreFooter> SpectreFooter { get; set; }
        public List<Feature> Features { get; set; }

    }
}
