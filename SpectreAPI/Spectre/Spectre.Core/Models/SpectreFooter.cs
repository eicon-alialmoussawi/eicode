using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class SpectreFooter
    {
        public int Id { get; set; }
        public string AddressEn { get; set; }
        public string AddressAr { get; set; }
        public string AddressFr { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string TitleFr { get; set; }
        public string BuildingEn { get; set; }
        public string BuildingAr { get; set; }
        public string BuildingFr { get; set; }
        public string StreetEn { get; set; }
        public string StreetAr { get; set; }
        public string StreetFr { get; set; }
        public string CityEn { get; set; }
        public string CityAr { get; set; }
        public string CityFr { get; set; }
    }
}
