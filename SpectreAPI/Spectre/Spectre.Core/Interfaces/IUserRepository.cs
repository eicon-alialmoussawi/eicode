using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<Tuple<bool, User>> Create(User User);
        Task<Tuple<bool, User>> Update(User User);
        Task<Tuple<bool, List<User>>> GetAll();
        Task<Tuple<bool, List<User>>> GetAllExistingUsers();
        Task<Tuple<bool, User>> GetByIdentityId(string IdentityId);
        Task<Tuple<bool, User>> GetById(int UserId);
        Task<Tuple<bool, User>> GetByEmail(string Email);
        Task<Tuple<bool, string>> GenerateRandomUserName();
        Task<Tuple<bool, List<Page>>> GetClientMenu(int UserId);
        Task<Tuple<bool, List<User_View2>>> GetUsersByCompanyId(int? CompanyId);
        Task<Tuple<bool>> DeleteCompanyUsers(string @Ids, int CompanyId, bool? IsActive);


        Task<Tuple<bool>> DeleteUser (int Id);
        Task<Tuple<bool>> UpdateUserLogInStatus(int Id);

        Task<Tuple<bool, UserInfo_View>> GetUserInfo(int UserId);
        Task<Tuple<bool, List<UserFeatures_View>>> GetUserFeaturs(int UserId, string Lang);
    }
}
