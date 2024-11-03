using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class ContactU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Message { get; set; }
        public int? CountryId { get; set; }
        public bool? IsDeleted { get; set; }
        public string PhoneNumber { get; set; }

        public virtual Country Country { get; set; }
    }
}
