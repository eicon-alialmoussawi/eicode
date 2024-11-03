using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBandRepository
    {
        Task<Band> GetById(int Id);
        Task<IEnumerable<Band>> GetAll();
        Task<Tuple<bool, Band>> Create(Band Band);
        Task<Tuple<bool, Band>> Update(Band Band);
        Task<Tuple<bool, bool>> SaveBand(Band Band);
        Task<Tuple<bool>> RemoveAllBands();
        Task<Tuple<bool>> DeleteBand(int Id);
    }
}
