using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Interfaces
{
    public interface ICompanyPreRegistrationRepository
    {
        Task<IEnumerable<CompanyPreRegistration>> GetAll();
        Task<Tuple<bool, List<CompanyPreRegistration_Details>>> GetAllDetails();
        Task<Tuple<bool, CompanyPreRegistration>> Create(CompanyPreRegistration PreRegistration);
        Task<Tuple<bool>> SetPreRegistrationAsViewed(int Id);
        Task<Tuple<bool>> DeletePreRegistration(int Id);
        Task<Tuple<bool, CompanyPreRegistration_Details>> GetPreRegistrationDetailsById(int Id);
    }
}
