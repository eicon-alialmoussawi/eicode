using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Interfaces
{
    public interface IStatisticRepository
    {
        Task<Tuple<bool, Statistic_View>> GetStatistics();
        Task<Tuple<bool, List<Sales_View>>> GetTotalSales();
        Task<Tuple<bool, List<CustomerPackage_View>>> GetTotalCustomersPerPackage();
        Task<Tuple<bool, ActivePackage_View>> ActivePackagesStatistics();
    }
}
