using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class SystemSetting
    {
        public int? Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AddressAr { get; set; }
        public string WelcomeMessage { get; set; }
        public string Fax { get; set; }
        public string AboutEn { get; set; }
        public string AboutAr { get; set; }
    }
}
