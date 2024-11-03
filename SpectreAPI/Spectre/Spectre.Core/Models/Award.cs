using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Award
    {
        public int Id { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? CountryId { get; set; }
        public int? OperatorId { get; set; }
        public decimal? PricePaid { get; set; }
        public string Terms { get; set; }
        public string BandPaired { get; set; }
        public string BandUnPaired { get; set; }
        public string BlockPaired { get; set; }
        public string BlockUnPaired { get; set; }
        public bool? RegionalLicense { get; set; }
        public decimal? Pop { get; set; }
        public string SingleOrmultiBand { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreatedBy { get; set; }
        public string BandType { get; set; }
        public decimal? UpFrontFees { get; set; }
        public decimal? AnnualFees { get; set; }
        public int? SourceId { get; set; }
        public string Operator { get; set; }
        public int? AuctionDateYear { get; set; }
        public int? AuctionDateMonth { get; set; }
        public decimal? ReservePrice { get; set; }

        public virtual Country Country { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual Lookup OperatorNavigation { get; set; }
        public virtual Lookup Source { get; set; }
    }
}
