using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IBazAwardRepository
    {
        Task<BazAward> GetById(int Id);
        Task<IEnumerable<BazAward>> GetAll();
        //Task<IEnumerable<BazAward>> GetAllCountryAuctions(int CountryId);
        Task<IEnumerable<BazAwardView>> GetAllForView();
        //Task<IEnumerable<BazAwardsFiltered>> FilterBazAwards(bool ISPPP, bool ISIMF, bool IsSingle, bool IsMultiple, bool IsPaired, bool IsPairedAndUnPaired,
        // bool RegionalLicense, int FromYear, int ToYear, int MaxGDP, int MinGDP, string CountryIds, string Band, int SourceId, bool IsUnPaired, int IssueDate);
        //Task<Tuple<bool, BazAward>> Create(BazAward BazAward);
        //Task<Tuple<bool, BazAward>> Update(BazAward BazAward);
        //Task<Tuple<bool, int>> CheckIfExists(BazAward BazAward);
        /*Task<IEnumerable<PricingFiltered>> FilterPricing(bool? IsPPP, bool? ISIMF, bool? IsPaired, bool? IsPairedAndUnPaired,
            bool? RegionalLicense,
            int? FromYear, int? ToYear, int? MaxGDP, int? MinGDP, string CountryIds, string Band, int? SourceId, int? IssueDate,
            int? Terms, decimal? DiscountRate,
            bool? IsIncludeAnnual, string SumBand, bool? UniqueAwards, bool? AverageAwards, bool? AnnualizePrice, bool? AdjustByPPPFactor,
            bool? AdjustByInflationFactor, bool? AdjustByGDPFactor, bool? AverageSumPricesAndMHZ);*/

        Task<Tuple<bool, List<BazAwardView>>> GetFilteredBazAwards(BazAwardFilter_View View);
        //Task<Tuple<bool>> RemoveAllBazAwards();
        //Task<Tuple<bool>> SaveBazAward(BazAward bazAward);

        //Task<IEnumerable<BazAwardsFiltered>> FilterBazAwards2(string Lang, bool IsPPP, bool IsIMF, int FromYear, int ToYear, int IssueDate, bool AdjustByPPP, bool AdjustByInflation,
        //string CountryIds, int UserId);
    }
}
