using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class UserNotification_View
    {
        public int Id { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public int? UserId { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public bool? Viewed { get; set; }
        public bool? Seen { get; set; }
        public bool? Important { get; set; }
        public bool? Expired { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsAutomated { get; set; }
        public string DescriptionEn { get; set; }
        public string DescritpionAr { get; set; }
        public string TextFr { get; set; }
        public string DescriptionFr { get; set; }
        public int? ViewedCount { get; set; }
        public int? SeenCount { get; set; }

    }
}
