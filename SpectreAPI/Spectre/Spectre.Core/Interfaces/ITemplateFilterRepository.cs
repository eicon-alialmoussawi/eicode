using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface ITemplateFilterRepository
    {
        Task<TemplateFilter> GetById(int Id);

        Task<IEnumerable<TemplateFilter>> GetAll();
        Task<IEnumerable<TemplateFilter>> GetAllByPage(string Page);

        
        Task<Tuple<bool, TemplateFilter>> Create(TemplateFilter TemplateFilter);
        Task<Tuple<bool, TemplateFilter>> Update(TemplateFilter TemplateFilter);


        Task<IEnumerable<TemplateFilterDetail>> GetDetailsByFilterId(int TemplateFilterId);


        Task<Tuple<bool, TemplateFilterDetail>> AddFilterDetails(TemplateFilterDetail TemplateFilterDetail);
    }
}
