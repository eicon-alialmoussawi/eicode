using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IOperatorRepository
    {
        Task<IEnumerable<Operator>> GetAll();
        Task<Operator> GetById(int id);
        Task<Tuple<bool, Operator>> Create(Operator _operator);
        Task<Tuple<bool, List<Operator>>> GetUserOperators(int userId, string pageUrl, string source, string lang);
    }
}

