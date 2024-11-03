using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Interfaces
{
    public interface ICompanyPackageRepository
    {
        Task<Tuple<bool, int>> Save(CompanyPackage_View companyView);
        Task<Tuple<bool, List<CompanyPackageD_View>>> GetAllWithDetails();
        Task<CompanyPackage> GetWithId(int Id);
        Task<Tuple<bool, Company_View>> GetCompanyById(int? CompanyId);
        Task<Tuple<bool, List<CompanyPackageDetails_View2>>> GetDetails(int CompanyPackageId);
        Task<Tuple<bool, int>> Update(CompanyPackageDetails companyView);
        Task<Tuple<bool, List<CompanyPackageD_View>>> GetExpiredCompanyPackages();
    }
}
