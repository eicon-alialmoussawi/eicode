using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazLicenseRepository
    {
        Task<IEnumerable<BazLicense>> GetAll();
        Task<BazLicense> GetById(int id);
        Task<Tuple<bool, BazLicense>> Create(BazLicense _operator);
    }
}
