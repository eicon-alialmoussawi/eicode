using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spectre.Core.Repositories
{
    public class AwardRepository : Repository<RegisterationRequest>, IAwardRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public AwardRepository(SpectreDBContext context, ILogger Logger, IConfiguration configuration)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, int>> CheckIfExists(Award Award)
        {
            var res = await MyDbContext.Awards
                       .Where(
                m => m.IsDeleted == false &&
                m.Year == Award.Year && m.Operator == Award.Operator && m.Month == Award.Month &&
                m.BandPaired == Award.BandPaired && m.BandUnPaired == Award.BandUnPaired &&
             m.CountryId == Award.CountryId && m.SingleOrmultiBand == Award.SingleOrmultiBand).AsNoTracking().FirstOrDefaultAsync();


            return Tuple.Create<bool, int>((res == null ? false : true), (res == null ? 0 : res.Id));
        }
        public async Task<Tuple<bool, Award>> Create(Award Award)
        {

            try
            {
                var Result = await MyDbContext.Awards.AddAsync(Award);
                MyDbContext.SaveChanges();

                MyDbContext.Entry(Award).State = EntityState.Detached;
                return new Tuple<bool, Award>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, Award>(false, null);
            }
        }
        public async Task<Tuple<bool, Award>> Update(Award Award)
        {

            try
            {
                MyDbContext.Entry(Award).State = EntityState.Modified;
                //var Result = MyDbContext.Awards.Update(Award);
                MyDbContext.SaveChanges();
                //MyDbContext.Entry(Award).State = EntityState.Detached;

                return new Tuple<bool, Award>(true, Award);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, Award>(false, null);
            }
        }
        public async Task<Award> GetById(int Id)
        {
            return await MyDbContext.Awards.Where(m => m.Id == Id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Award>> GetAll()
        {
            return await MyDbContext.Awards.Where(m => m.IsDeleted == false).ToListAsync();
        }
        public async Task<IEnumerable<Award>> GetAllCountryAuctions(int CountryId)
        {
            return await MyDbContext.Awards.Where(m => m.IsDeleted == false && m.CountryId == CountryId &&
            ((m.BlockPaired != null || m.BlockPaired != "") && (m.BlockUnPaired != null || m.BlockUnPaired != ""))
            && m.Terms != null && m.Terms != "" && m.UpFrontFees != null && m.UpFrontFees > 0).ToListAsync();
        }

        public async Task<IEnumerable<AwardView>> GetAllForView()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    var Results = await Connection.QueryAsync<AwardView>("GetAllAwards", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<AwardsFiltered>> FilterAwards(bool ISPPP, bool ISIMF, bool IsSingle, bool IsMultiple, bool IsPaired, bool IsPairedAndUnPaired,
            bool RegionalLicense, int FromYear, int ToYear, int MaxGDP, int MinGDP, string CountryIds, string Band, int SourceId, bool IsUnPaired, int IssueDate)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@ISPPP", ISPPP);
                    Params.Add("@ISIMF", ISIMF);
                    Params.Add("@IsSingle", IsSingle);
                    Params.Add("@IsMultiple", IsMultiple);
                    Params.Add("@IsPaired", IsPaired);
                    Params.Add("@IsPairedAndUnPaired", IsPairedAndUnPaired);
                    Params.Add("@RegionalLicense", RegionalLicense);
                    Params.Add("@FromYear", FromYear);
                    Params.Add("@IsUnPaired", IsUnPaired);

                    Params.Add("@ToYear", ToYear);
                    Params.Add("@MaxGDP", MaxGDP);
                    Params.Add("@MinGDP", MinGDP);
                    Params.Add("@CountryIds", CountryIds);
                    Params.Add("@Band", Band);
                    Params.Add("@SourceId", SourceId);
                    Params.Add("@IssueDate", IssueDate);
                    var Results = await Connection.QueryAsync<AwardsFiltered>("FilterAwards", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }

        public class XYPoint
        {
            public int X;
            public double Y;
        }
        public static List<XYPoint> GenerateLinearBestFit(List<XYPoint> points, out double a, out double b)
        {
            int numPoints = points.Count;
            double meanX = points.Average(point => point.X);
            double meanY = points.Average(point => point.Y);

            double sumXSquared = points.Sum(point => point.X * point.X);
            double sumXY = points.Sum(point => point.X * point.Y);

            a = (sumXY / numPoints - meanX * meanY) / (sumXSquared / numPoints - meanX * meanX);
            b = (a * meanX - meanY);

            double a1 = a;
            double b1 = b;

            return points.Select(point => new XYPoint() { X = point.X, Y = a1 * point.X - b1 }).ToList();
        }



        public async Task<IEnumerable<PricingFiltered>> FilterPricing(bool? IsPPP, bool? ISIMF, bool? IsPaired, bool? IsPairedAndUnPaired, bool? RegionalLicense, int? FromYear, int? ToYear, int? MaxGDP, int? MinGDP, string CountryIds, string Band, int? SourceId, int? IssueDate, int? Terms, decimal? DiscountRate, bool? IsIncludeAnnual, string SumBand, bool? UniqueAwards, bool? AverageAwards, bool? AnnualizePrice, bool? AdjustByPPPFactor,
            bool? AdjustByInflationFactor, bool? AdjustByGDPFactor, bool? AverageSumPricesAndMHZ)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@IsPPP", IsPPP);
                    Params.Add("@ISIMF", ISIMF);
                    Params.Add("@IsPaired", IsPaired);
                    Params.Add("@IsPairedAndUnPaired", IsPairedAndUnPaired);
                    Params.Add("@RegionalLicense", RegionalLicense);
                    Params.Add("@FromYear", FromYear);
                    Params.Add("@ToYear", ToYear);
                    Params.Add("@MaxGDP", MaxGDP);
                    Params.Add("@MinGDP", MinGDP);
                    Params.Add("@CountryIds", CountryIds);
                    Params.Add("@Band", Band);
                    Params.Add("@SourceId", SourceId);
                    Params.Add("@IssueDate", IssueDate);
                    Params.Add("@Terms", Terms);
                    Params.Add("@DiscountRate", DiscountRate);
                    Params.Add("@IsIncludeAnnual", IsIncludeAnnual);
                    Params.Add("@SumBand", SumBand);

                    Params.Add("@UniqueAwards", UniqueAwards);
                    Params.Add("@AverageAwards", AverageAwards);
                    Params.Add("@AnnualizePrice", AnnualizePrice);
                    Params.Add("@AdjustByPPPFactor", AdjustByPPPFactor);
                    Params.Add("@AdjustByInflationFactor", AdjustByInflationFactor);
                    Params.Add("@AdjustByGDPFactor", AdjustByGDPFactor);
                    Params.Add("@AverageSumPricesAndMHZ", AverageSumPricesAndMHZ);


                    var Results = await Connection.QueryAsync<PricingFiltered>("FilterPricing", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }

        }

        public async Task<Tuple<bool, List<AwardView>>> GetFilteredAwards(AwardFilter_View View)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@CountryIds", View.CountryIds);
                    Params.Add("@BandTypes", View.BandTypes);
                    Params.Add("@BandUnPaired", View.BandUnPaired);
                    Params.Add("@OperatorName", View.OperatorName);
                    Params.Add("@Month", View.Month);
                    Params.Add("@Year", View.Year);

                    var Results = await Connection.QueryAsync<AwardView>("GetFilteredAwards", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool, List<AwardView>>(true, Results.ToList());
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<AwardView>>(false, null);
            }
        }

        public async Task<Tuple<bool>> RemoveAllAwards()
        {

            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();


                    var Results = await Connection.ExecuteAsync("RemoveAllAwards", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool>(true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool>(false);
            }
        }

        public async Task<Tuple<bool>> SaveAward(Award award)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Id", award.Id);
                    Params.Add("@Year", award.Year);
                    Params.Add("@Month", award.Month);
                    Params.Add("@CountryId", award.CountryId);
                    Params.Add("@OperatorId", award.OperatorId);
                    Params.Add("@PricePaid", award.PricePaid);
                    Params.Add("@Terms", award.Terms);
                    Params.Add("@BandPaired", award.BandPaired);
                    Params.Add("@BandUnPaired", award.BandUnPaired);

                    Params.Add("@BlockPaired", award.BlockPaired);
                    Params.Add("@BlockUnPaired", award.BlockUnPaired);
                    Params.Add("@RegionalLicense", award.RegionalLicense);
                    Params.Add("@Pop", award.Pop);
                    Params.Add("@SingleOrmultiBand", award.SingleOrmultiBand);
                    Params.Add("@IsDeleted", award.IsDeleted);
                    Params.Add("@CreationDate", award.CreationDate);

                    Params.Add("@CreatedBy", award.CreatedBy);
                    Params.Add("@BandType", award.BandType);
                    Params.Add("@UpFrontFees", award.UpFrontFees);
                    Params.Add("@AnnualFees", award.AnnualFees);
                    Params.Add("@SourceId", award.SourceId);
                    Params.Add("@Operator", award.Operator);
                    Params.Add("@AuctionDateYear", award.AuctionDateYear);
                    Params.Add("@AuctionDateMonth", award.AuctionDateMonth);
                    Params.Add("@ReservePrice", award.ReservePrice);
                    var Results = await Connection.ExecuteAsync("SaveAwards", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool>(true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool>(false);
            }
        }

        public async Task<IEnumerable<AwardsFiltered>> FilterAwards2(string Lang, bool IsPPP, bool IsIMF, int FromYear, int ToYear, int IssueDate, bool AdjustByPPP, bool AdjustByInflation, string CountryIds, int UserId)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@Lang", Lang);
                    Params.Add("@IsPPP", IsPPP);
                    Params.Add("@IsIMF", IsIMF);
                    Params.Add("@FromYear", FromYear);
                    Params.Add("@ToYear", ToYear);
                    Params.Add("@IssueDate", IssueDate);
                    Params.Add("@AdjustByPPP", AdjustByPPP);
                    Params.Add("@AdjustByInflation", AdjustByInflation);
                    Params.Add("@CountryIds", CountryIds);
                    Params.Add("@UserId", UserId);

                    var Results = await Connection.QueryAsync<AwardsFiltered>("FilterAwards2", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return Results.ToList();
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return null;
            }
        }
    }
}
