using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country> GetById(int Id);

        Task<Country> GetByCode(string Code);
        Task<IEnumerable<Country>> GetAll();
        Task<Country> getByIso3(string code);
        Task<Tuple<bool, Country>> Create(Country Country);
        Task<Tuple<bool, List<Country_View>>> GetUserCountries(int UserId, string PageUrl, string Source, string Lang);

    }
}
