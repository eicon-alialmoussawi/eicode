using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface ILatestNewsRepository
    {
        Task<LatestNews> GetById(int Id);
        Task<IEnumerable<LatestNews>> GetAll();
        Task<Tuple<bool, LatestNews>> Create(LatestNews LatestNews);
        Task<Tuple<bool, LatestNews>> Update(LatestNews LatestNews);
        Task<IEnumerable<LatestNews>> GetPublishedNews();
    }
}
