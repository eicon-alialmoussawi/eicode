using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;

namespace Spectre.Core.Repositories
{
    public interface IFeatureRepository
    {
        Task<Tuple<bool, List<Feature>>> GetAll();
        Task<Tuple<bool>> Save(Feature Feature);
        Task<Tuple<bool>> Delete(int Id);
    }
}
