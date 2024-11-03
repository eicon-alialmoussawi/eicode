using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spectre.Core.Interfaces
{
  public  interface IScocioEconomicRepository
    {
        Task<SocioEonomic> GetById(int Id);
        Task<SocioEonomic> GetByCountryAndYear(int CountryId, int Year, bool ISIMF);
        Task<IEnumerable<SocioEonomic>> GetAll();
        Task<IEnumerable<SocioEonomic_View>> GetAllSocioEconomics();
        Task<IEnumerable<SocioEonomic_View2>> FilterSocioEconimcs(string CountryIds, int SourceId);
        Task<IEnumerable<SocioEonomic_View2>> FilterSocioEconimcsByCountry(int CountryId);
        Task<IEnumerable<SoioEconomicColumnHeader>> FilterSocioEconimcsByCountryAndSource(int CountryId,int SourceId);
        Task<Tuple<bool, SocioEonomic>> Create(SocioEonomic SocioEonomic);
        Task<Tuple<bool, SocioEonomic>> Update(SocioEonomic SocioEonomic);
        Task<Tuple<bool,int>> CheckIfExists(SocioEonomic SocioEonomic);
        Task<Tuple<bool>> DeleteAllSocioEcnomic();
        Task<Tuple<bool, decimal?>> GetGDPForValuations(int Year, int CountryId, bool IsPPP, bool IsIMF);


        Task<Tuple<bool, decimal?>> GetPopForValuations(int Year, int CountryId, bool IsIMF);

        Task<Tuple<bool, List<SocioEcnomic_YearView>>> FilterSocioEconimcsByYear(int Year, bool? IsIMF);
    }
}
