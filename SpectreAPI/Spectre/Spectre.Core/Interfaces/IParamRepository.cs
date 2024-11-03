using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;

namespace Spectre.Core.Interfaces
{
    public interface IParamRepository
    {
        Task<Tuple<bool, List<Parameter>>> GetAll();
        Task<Tuple<bool, bool>> Update(int ParamId, string Value);
        Task<Tuple<bool, Parameter>> GetById(int ParamId);
    }
}
