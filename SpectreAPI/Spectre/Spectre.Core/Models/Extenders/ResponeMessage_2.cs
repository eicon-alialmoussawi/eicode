using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class ResponeMessage_2
    {
        public bool Success { get; set; }
        public List<UnacceptedUsers_View> users { get; set; }
    }
}
