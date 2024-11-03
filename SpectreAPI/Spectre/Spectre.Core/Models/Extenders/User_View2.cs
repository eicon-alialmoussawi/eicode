using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class User_View2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? CreationDate { get; set; }
        public string IdentityUserId { get; set; }
        public bool? IsDeleted { get; set; }
        public string LastName { get; set; }
        public string IdentityNumber { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsLocked { get; set; }
        public int? CompanyId { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }


    }
}
