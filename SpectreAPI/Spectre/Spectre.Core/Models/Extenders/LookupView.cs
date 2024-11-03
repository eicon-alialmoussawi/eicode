using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class LookupView
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string LookupCode { get; set; }
        public string Name { get; set; }
        public int? Order { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModiciationDate { get; set; }
        public int? AccountId { get; set; }
        public string Description { get; set; }
        public bool? IsDefault { get; set; }
        public bool? UserDefined { get; set; }
        public string NameAr { get; set; }
        public string ParentName { set; get; }
    }
}
