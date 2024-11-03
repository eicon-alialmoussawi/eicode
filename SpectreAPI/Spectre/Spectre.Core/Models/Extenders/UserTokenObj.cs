using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class UserTokenObj
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string IdentityUser { get; set; }
        public string Lang { get; set; }
        public bool IsAdmin { get; set; }
    }
}
