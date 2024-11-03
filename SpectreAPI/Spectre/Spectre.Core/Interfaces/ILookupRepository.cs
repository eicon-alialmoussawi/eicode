using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface ILookupRepository
    {
        Task<IEnumerable<Lookup>> GetLookupsByParantCode(string code);
        Task<IEnumerable<Lookup>> GetAll();

        Task< Lookup> GetById(int Id);
        Task<Lookup> GetByName(string Name);
        Task<Lookup> GetLookupByCode(string code);
        Task<Lookup> GetLookupById(int id);



        Task<Tuple<bool, Lookup>> Create(Lookup Lookup);
        Task<Tuple<bool, Lookup>> Update(Lookup Lookup);
        Task<Tuple<bool, List<Lookup>>> GetLookupsBySocialCode(bool IsIMF);
    }
}
