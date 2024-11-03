using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class LoginResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? LoggedInBefore { get; set; }
        public string Lang { set; get; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string CroppedImageURL { get; set; }
        public string NameAr { set; get; }
        public string LastNameAr { set; get; }
        public int? AllowClientToChooseBePositive { get; set; }
        
    }
}
