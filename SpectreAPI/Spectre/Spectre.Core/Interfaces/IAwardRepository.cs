using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spectre.Core.Interfaces
{
    public interface IAwardRepository
    {
        Task<Award> GetById(int Id);
        Task<IEnumerable<Award>> GetAll();
        Task<IEnumerable<Award>> GetAllCountryAuctions(int CountryId);
        Task<IEnumerable<AwardView>> GetAllForView();
        Task<IEnumerable<AwardsFiltered>> FilterAwards(bool ISPPP, bool ISIMF, bool IsSingle, bool IsMultiple,bool IsPaired, bool IsPairedAndUnPaired,
            bool RegionalLicense, int FromYear, int ToYear, int MaxGDP, int MinGDP, string CountryIds, string Band, int SourceId, bool IsUnPaired,int IssueDate);
        Task<Tuple<bool, Award>> Create(Award Award);
        Task<Tuple<bool, Award>> Update(Award Award);
        Task<Tuple<bool, int>> CheckIfExists(Award Award);
        Task<IEnumerable<PricingFiltered>> FilterPricing(bool? IsPPP, bool? ISIMF, bool? IsPaired, bool? IsPairedAndUnPaired,
            bool? RegionalLicense,
            int? FromYear, int? ToYear, int? MaxGDP, int? MinGDP, string CountryIds, string Band, int? SourceId, int? IssueDate, 
            int? Terms, decimal? DiscountRate,
            bool? IsIncludeAnnual, string SumBand , bool?UniqueAwards, bool? AverageAwards, bool? AnnualizePrice, bool? AdjustByPPPFactor, 
            bool? AdjustByInflationFactor, bool? AdjustByGDPFactor, bool? AverageSumPricesAndMHZ);

        Task<Tuple<bool, List<AwardView>>> GetFilteredAwards(AwardFilter_View View);
        Task<Tuple<bool>> RemoveAllAwards();
        Task<Tuple<bool>> SaveAward(Award award);

        Task<IEnumerable<AwardsFiltered>> FilterAwards2(string Lang, bool IsPPP, bool IsIMF, int FromYear, int ToYear, int IssueDate, bool AdjustByPPP, bool AdjustByInflation,
            string CountryIds, int UserId);

    }
}
