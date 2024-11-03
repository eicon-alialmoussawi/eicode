using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IRegionRepository
    {
        Task<Tuple<bool, RegionDetails>> GetById(int Id);
        Task<Tuple<bool, RegionDetails>> Create(RegionDetails Region);
        Task<Tuple<bool, List<RegionDetails>>> GetRegions();
        Task<Tuple<bool>> Delete(int Id);

    }
}
