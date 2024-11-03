using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class LoginDetails
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
