using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{

    public class LoginUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
        public string IdentityUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        public string UserType { get; set; }
        public string ProfileImageURL { get; set; }
        public string CroppedImageURL { get; set; }
        public string JobTitle { get; set; }
    }
}
