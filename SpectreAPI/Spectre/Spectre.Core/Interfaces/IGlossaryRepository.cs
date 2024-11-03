using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;

namespace Spectre.Core.Interfaces
{
    public interface IGlossaryRepository
    {
        Task<IEnumerable<Glossary>> GetAll();
        Task<Tuple<bool>> Save(Glossary Glossary);
        Task<Tuple<bool>> Delete(int Id);
    }
}
