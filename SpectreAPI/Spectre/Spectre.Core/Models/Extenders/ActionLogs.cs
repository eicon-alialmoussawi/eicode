using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{

    public class ActionLogs
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Page { get; set; }
        public string Details { get; set; }
        public DateTime Date { get; set; }
    }
}
