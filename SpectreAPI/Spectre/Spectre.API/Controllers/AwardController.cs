using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.API.Utilities;
using Spectre.Core.Interfaces;

using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;

namespace Spectre.API.Controllers
{
    [Route("api/Award")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AwardController : ControllerBase
    {
        public class XYPoint
        {
            public decimal X;
            public decimal Y;
            public int Id;
            public decimal Price;
        }

        public class XYPoint2
        {
            public decimal X;
            public double Y;
            public int Id;
            public double Price;
        }

        private readonly IMapper mapper;
        private readonly IAwardRepository IAwardRepository;
        private readonly ILogger ILogger;
        public readonly ICountryRepository ICountryRepository;
        public readonly ILookupRepository ILookupRepository;
        public readonly IScocioEconomicRepository IScocioEconomicRepository;

        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public AwardController(IHostingEnvironment _hostingEnvironment, IScocioEconomicRepository IScocioEconomicRepository, IMapper mapper, ILogger ILogger, IAwardRepository IAwardRepository, ICountryRepository ICountryRepository, ILookupRepository ILookupRepository)
        {
            this.IAwardRepository = IAwardRepository;
            this.ILookupRepository = ILookupRepository;
            this.ICountryRepository = ICountryRepository;
            this.IScocioEconomicRepository = IScocioEconomicRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
            this._hostingEnvironment = _hostingEnvironment;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Award Award)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            if (Award.Id == 0)
            {
                Award.CreationDate = DateTime.Now;
                Award.CreatedBy = UserId;
                var Result = await IAwardRepository.SaveAward(Award);
                if (!Result.Item1)
                    return BadRequest();
                else
                {

                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),

                        Page = "Award",
                        PageAr = "جائزة",
                        Details = ("Award  for Year(" + Award.Year + ") is added"),
                        DetailsAr = ("تم اضافة  جائزة (" + Award.Year + ")"),
                        Date = DateTime.Now
                    });



                    return Ok(Result.Item1);
                }
            }
            else
            {

                var Result = await IAwardRepository.SaveAward(Award);

                if (!Result.Item1)
                    return BadRequest();
                else
                {
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),
                        Page = "Award",
                        PageAr = "جائزة",
                        Details = ("Award  for Year(" + Award.Year + ") is updated"),
                        DetailsAr = ("تم تعديل  جائزة (" + Award.Year + ")"),
                        Date = DateTime.Now
                    });

                }

                return Ok(Result.Item1);
            }
        }

        [HttpPost]
        [Route("ImportAwards")]
        public async Task<IActionResult> ImportAwards(List<AwardImportBody> Awards)
        {
            List<FailedImportedAwards_View> result = new List<FailedImportedAwards_View>();

            foreach (AwardImportBody AwardImportBody in Awards)
            {
                try
                {
                    //check if country and  year and month exists
                    Award award = new Award();
                    award.Id = 0;
                    award.Year = AwardImportBody.Year;
                    award.Month = AwardImportBody.Month == "" ? null : DateTime.ParseExact(AwardImportBody.Month, "MMM", CultureInfo.CurrentCulture).Month;
                    award.IsDeleted = false;
                    award.Pop = AwardImportBody.Pop;
                    award.SingleOrmultiBand = AwardImportBody.IsSingleOrMulti;
                    award.Terms = AwardImportBody.Terms;
                    if (AwardImportBody.AuctionDateMonth != null && AwardImportBody.AuctionDateMonth != "")
                        award.AuctionDateMonth = DateTime.ParseExact(AwardImportBody.AuctionDateMonth, "MMM", CultureInfo.CurrentCulture).Month;
                    award.AuctionDateYear = AwardImportBody.AuctionDateYear;
                    award.ReservePrice = AwardImportBody.ReservePrice;

                    if (AwardImportBody.ISO != null && AwardImportBody.ISO != "")
                    {
                        var country = await ICountryRepository.getByIso3(AwardImportBody.ISO);
                        if (country != null)
                            award.CountryId = country.CountryId;
                    }

                    award.Operator = AwardImportBody.Operator;
                    award.UpFrontFees = AwardImportBody.UpFrontFees;
                    award.AnnualFees = AwardImportBody.AnnualFees;
                    award.BandPaired = AwardImportBody.BandPaired;
                    award.BandUnPaired = AwardImportBody.BandUnPaired;
                    if (AwardImportBody.BlockPaired != "")
                        award.BlockPaired = (AwardImportBody.BlockPaired);
                    if (AwardImportBody.BlockUnPaired != "")
                        award.BlockUnPaired = (AwardImportBody.BlockUnPaired);
                    award.BandType = AwardImportBody.IsSingleOrMulti;
                    award.RegionalLicense = Convert.ToBoolean(Convert.ToInt16(AwardImportBody.RegionalLicense));
                    award.CreationDate = DateTime.Now;

                    //var check = await IAwardRepository.CheckIfExists(award);

                    //if (check.Item1)
                    //{
                    //    award.Id = check.Item2;

                    //    var Res = await IAwardRepository.Update(award);
                    //}
                    //else
                    //{
                    var Res = await IAwardRepository.SaveAward(award);
                    //  }
                }
                catch (Exception e)
                {
                    FailedImportedAwards_View item = new FailedImportedAwards_View();
                    item.Country = AwardImportBody.Country;
                    item.Month = AwardImportBody.Month;
                    item.Year = AwardImportBody.Year;
                    item.Value = 0;

                    result.Add(item);

                }
            }

            return Ok(result);
        }


        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> Import(List<Award> Awards)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            foreach (Award award in Awards)
            {

                if (award.Id == 0)
                {


                    var Result = await IAwardRepository.Create(award);

                    if (!Result.Item1)
                        return BadRequest();
                    else
                    {
                        await ILogger.LogUserAction(new UserActionLog
                        {
                            Id = 0,
                            UserId = UserId,
                            Action = Convert.ToInt32(Operations.Update),

                            Page = "Award",
                            PageAr = "جائزة",
                            Details = ("Award  for Year(" + award.Year + ") is added"),
                            DetailsAr = ("تم اضافة  جائزة (" + award.Year + ")"),
                            Date = DateTime.Now
                        });


                    }
                }
                else
                {

                    var Result = await IAwardRepository.Update(award);

                    if (!Result.Item1)
                        return BadRequest();
                    else
                    {
                        await ILogger.LogUserAction(new UserActionLog
                        {
                            Id = 0,
                            UserId = UserId,
                            Action = Convert.ToInt32(Operations.Update),
                            Page = "Award",
                            PageAr = "جائزة",
                            Details = ("Award  for Year(" + award.Year + ") is updated"),
                            DetailsAr = ("تم تعديل  جائزة (" + award.Year + ")"),
                            Date = DateTime.Now
                        });



                    }
                }
            }
            return Ok("Successfully done");
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IAwardRepository.GetAll();
            return Ok(Result);
        }

        [HttpPost]
        [Route("Filter")]
        public async Task<IActionResult> FilterAwards(AwardsFilterBody AwardsFilterBody)
        {

            // Getting the list of awards for respective selected countries, from and to year
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IAwardRepository.FilterAwards2(AwardsFilterBody.Lang, AwardsFilterBody.ISPPP, AwardsFilterBody.ISIMF, AwardsFilterBody.FromYear, AwardsFilterBody.ToYear,
                1, false, false, AwardsFilterBody.CountryIds, UserId);

            // If user doesnt select regional then we must get all non regional awardss
            if (AwardsFilterBody.RegionalLicense == false)
                Result = Result.Where(m => m.Regionalicense == false).ToList();

            // if user selects single and multi band then we must double check that award band type has s or i or m
            if (AwardsFilterBody.IsSingle && AwardsFilterBody.IsMultiple)
            {
                Result = Result.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i' || m.SingleOrMultiBand == 'm' || m.SingleOrMultiBand == 'M').ToList();

            }
            else
            {
                // filter single awards only
                if (AwardsFilterBody.IsSingle == true)
                {
                    Result = Result.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i').ToList();
                }
                //filter muultiple awards only
                if (AwardsFilterBody.IsMultiple == true)
                {
                    Result = Result.Where(m => m.SingleOrMultiBand == 'm' || m.SingleOrMultiBand == 'M').ToList();
                }
            }
            // get the list of user selected bands in array and then check what pairing type he checks
            string[] SelectedBands = AwardsFilterBody.Band.Split(',');

            var singleResult = Result.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i').ToList();
            var FilteredResults = new List<AwardsFiltered>();
            // if he select single bands only
            if (AwardsFilterBody.IsSingle)
            {
                // check if we have to compare than band whether in the paired if its selected
                if (AwardsFilterBody.IsPaired)
                {
                    foreach (AwardsFiltered Award in singleResult)
                    {
                        if (SelectedBands.Contains(Award.BandPaired))
                            FilteredResults.Add(Award);
                    }
                }
                // check if we have to compare than band whether in the unpaired if its selected
                if (AwardsFilterBody.IsUnPaired)
                {
                    foreach (AwardsFiltered Award in singleResult)
                    {
                        if (SelectedBands.Contains(Award.BandUnPaired) && !FilteredResults.Contains(Award))
                            FilteredResults.Add(Award);
                    }
                }


                // check if we have to compare than band whether in the paired+unpaired option if its selected
                if (AwardsFilterBody.IsPairedAndUnPaired)
                {
                    foreach (AwardsFiltered Award in singleResult)
                    {
                        if (SelectedBands.Contains(Award.BandUnPaired) && SelectedBands.Contains(Award.BandPaired))
                            if (!FilteredResults.Contains(Award))
                                FilteredResults.Add(Award);
                    }
                    //    FilteredResults.AddRange(Result.Where(m => SelectedBands.Contains(m.BandUnPaired) && SelectedBands.Contains(m.BandPaired)).ToList());

                }
                //    Result = FilteredResults;
            }


            //if user selected multi band than we must take combinations from the bands selected and check if they match bands
            // found in the awards bandpaired or bandunpaired column
            var FilteredResults2 = new List<AwardsFiltered>();
            var multiBands = Result.Where(m => m.SingleOrMultiBand == 'm' || m.SingleOrMultiBand == 'M').ToList();
            if (AwardsFilterBody.IsMultiple)
            {
                foreach (AwardsFiltered Award in multiBands)
                {
                    string[] AwardBandPaired = Award.BandPaired.Split("-");
                    string[] AwardBandUnPaired = Award.BandUnPaired.Split("-");

                    bool test = CheckIfContains(SelectedBands, AwardBandPaired);
                    bool test2 = CheckIfContains(SelectedBands, AwardBandUnPaired);

                    if (AwardBandPaired.Length > 0 && AwardBandUnPaired.Length > 0 && Award.BandPaired != "" && Award.BandUnPaired != "")
                    {

                        if (!FilteredResults2.Contains(Award) && !FilteredResults.Contains(Award) && (test && test2))
                            FilteredResults2.Add(Award);
                    }
                    else
                    {
                        if (!FilteredResults2.Contains(Award) && !FilteredResults.Contains(Award) && (test || test2))
                            FilteredResults2.Add(Award);
                    }
                }

            }

            FilteredResults.AddRange(FilteredResults2);
            Result = FilteredResults;
            if (!AwardsFilterBody.IsPairedAndUnPaired)
                Result = Result.Where(m => m.Pairing != "p+u").ToList();
            if (!AwardsFilterBody.IsPaired)
                Result = Result.Where(m => m.Pairing != "p").ToList();
            if (!AwardsFilterBody.IsUnPaired)
                Result = Result.Where(m => m.Pairing != "u").ToList();

            if (AwardsFilterBody.MinGDP >= 0 && AwardsFilterBody.MaxGDP >= 0)
                Result = Result.Where(m => (m.GDP == null || m.GDP >= AwardsFilterBody.MinGDP) && (m.GDP == null || m.GDP <= AwardsFilterBody.MaxGDP)).ToList();

            Result = Result.Where(m => (m.MHZ != null && m.MHZ != "")).ToList();

            List<AwardsFiltered> FinalResult = new List<AwardsFiltered>();
            foreach (AwardsFiltered AwardsFiltered in Result)
            {
                if (AwardsFiltered.Pairing == "p+u" && (AwardsFiltered.BlockPaired == null || AwardsFiltered.BlockUnPaired == null))
                    continue;
                else
                {
                    if (AwardsFiltered.Regionalicense)
                        AwardsFiltered.Pop = AwardsFiltered.AwardPop;

                    if (AwardsFiltered.MHZ.EndsWith("-"))
                    {
                        AwardsFiltered.MHZ = AwardsFiltered.MHZ.Substring(0, AwardsFiltered.MHZ.Length - 1);
                    }

                    if (AwardsFiltered.Band.EndsWith("-"))
                    {
                        AwardsFiltered.Band = AwardsFiltered.Band.Substring(0, AwardsFiltered.Band.Length - 1);
                    }

                    FinalResult.Add(AwardsFiltered);
                }

                if(AwardsFiltered.coverage == "Regional" && AwardsFilterBody.ISIMF == true)
                {
                    AwardsFiltered.Pop = AwardsFiltered.Pop / 1000000;
                }

            }


            return Ok(FinalResult);

        }
        public static bool CheckIfContains(string[] arr1, string[] arr2)
        {
            bool flag = false;
            if (arr1.Length < arr2.Length)
                return false;
            for (int i = 0; i < arr2.Length; i++)
            {
                if (!arr1.Contains(arr2[i]))
                {
                    flag = false;
                    break;
                }
                flag = true;
            }
            return flag;
        }

        public static List<XYPoint2> GenerateLinearBestFit(List<XYPoint2> points, out double a, out double b)
        {
            try
            {
                int numPoints = points.Count;
                if(numPoints > 0)
                {
                    double meanX = points.Average(point => Convert.ToDouble(point.X));
                    double meanY = points.Average(point => Convert.ToDouble(point.Y));

                    double sumXSquared = points.Sum(point => Convert.ToDouble(point.X * point.X));
                    double sumXY = points.Sum(point => Convert.ToDouble(point.X) * Convert.ToDouble(point.Y));

                    a = (sumXY / numPoints - meanX * meanY) / (sumXSquared / numPoints - meanX * meanX);
                    //  b = (a * meanX - meanY);
                    b = meanY - (a * meanX);

                    double a1 = a;
                    double b1 = b;

                    return points.Select(point => new XYPoint2() { X = point.X, Y = a1 * Convert.ToDouble(point.X) + b1, Id = point.Id, Price = point.Price }).ToList();
                }
                else
                {
                    double a1 = 0;
                    double b1 = 0;
                    a = 0;
                    b = 0;
                    return new List<XYPoint2>();
                }
            }
            catch (Exception ex)
            {
                double a1 = 0;
                double b1 = 0;
                a = 0;
                b = 0;
                return points.Select(point => new XYPoint2() { X = point.X, Y = point.Y, Id = point.Id, Price = point.Price }).ToList();

            }
        }


        public static List<XYPoint2> GenerateLinearBestFit2(List<XYPoint2> points, string EnforceBPositive, out double a, out double b, out string Way)
        {
            try
            {
                int numPoints = points.Count;
                double meanX = points.Average(point => Convert.ToDouble(point.X));
                double meanY = points.Average(point => Convert.ToDouble(point.Y));

                double sumX = points.Sum(point => Convert.ToDouble(point.X));
                double sumY = points.Sum(point => point.Y);

                double sumXSquared = points.Sum(point => Convert.ToDouble(point.X * point.X));
                double sumXY = points.Sum(point => Convert.ToDouble(point.X) * point.Y);


                double sumXYWithMean = points.Sum(point => ((Convert.ToDouble(point.X) - meanX) * (point.Y - meanY)));


                double sumXWithMean = points.Sum(point => (Math.Pow((double)point.X - (double)meanX, 2)));
                double a1;
                double b1;

                a = sumXYWithMean / (double)sumXWithMean;
                ///  a = (sumXY / numPoints - meanX * meanY) / (sumXSquared / numPoints - meanX * meanX);
                //  b = (a * meanX - meanY);
                b = meanY - (a * meanX);

                if (EnforceBPositive=="1")
                {
                    if (a > 0)
                    {
                        a1 = a;
                        b1 = b;
                        if (b > 0)
                        {
                            Way = "Normal";
                            return points.Select(point => new XYPoint2() { X = point.X, Y = a1 * Convert.ToDouble(point.X) + b1, Id = point.Id, Price = point.Price }).ToList();
                        }

                        else
                        {
                            a = (sumXY) / (sumXSquared);
                            b = 0;

                            a1 = a;
                            b1 = b;

                            Way = "Modified";
                            return points.Select(point => new XYPoint2() { X = point.X, Y = a1 * Convert.ToDouble(point.X) + b1, Id = point.Id, Price = point.Price }).ToList();


                        }
                    }
                    else
                    {
                        a = (sumXY) / (sumXSquared);
                        b = 0;

                        a1 = a;
                        b1 = b;
                        Way = "Modified";
                        return points.Select(point => new XYPoint2() { X = point.X, Y = a1 * Convert.ToDouble(point.X) + b1, Id = point.Id, Price = point.Price }).ToList();
                    }
                    
                }
                else if (EnforceBPositive == "0")
                {
                    a1 = a;
                    b1 = b;
                        Way = "Normal";
                        return points.Select(point => new XYPoint2() { X = point.X, Y = a1 * Convert.ToDouble(point.X) + b1, Id = point.Id, Price = point.Price }).ToList();
                }
                else
                {
                    a = (sumXY) / (sumXSquared);
                    b = 0;

                    a1 = a;
                    b1 = b;

                    Way = "Modified";
                    return points.Select(point => new XYPoint2() { X = point.X, Y = a1 * Convert.ToDouble(point.X) + b1, Id = point.Id, Price = point.Price }).ToList();

                }
            }
            catch (Exception ex)
            {
                decimal a1 = 0;
                decimal b1 = 0;
                a = 0;
                b = 0;
                Way = "Modified";
                return points.Select(point => new XYPoint2() { X = point.X, Y = point.Y, Id = point.Id, Price = point.Price }).ToList();

            }
        }


        public static double GetBandTerm(string Terms, string Bands, string ItemBand)
        {
            double Res = 0;
            string[] SelectedBands = Bands.Split(',');
            string[] SelectedTerms = Terms.Split(',');
            for (int i = 0; i < SelectedBands.Length; i++)
            {
                if (SelectedBands[i] == ItemBand)
                {
                    Res = Double.Parse(SelectedTerms[i]);
                    break;
                }
            }
            return Res;

        }

        [HttpPost]
        [Route("FilterPricing")]
        public async Task<IActionResult> FilterPricing(PricingFilterBody PricingFilterBody)
        {

            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
                PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor, PricingFilterBody.AdjustByInflationFactor,
                PricingFilterBody.CountryIds, UserId);


            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
            && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
            && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();



            List<AwardsFiltered> filtered = new List<AwardsFiltered>();
            foreach (AwardsFiltered item in Result)
            {
                if (item.Pairing == "p")
                {
                    if (item.BlockPaired != "" && item.BlockPaired != null)
                        filtered.Add(item);
                }
                else if (item.Pairing == "u")
                {
                    if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                        filtered.Add(item);
                }
                else if (item.Pairing == "p+u")
                {
                    if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                        item.BlockPaired != "")
                        filtered.Add(item);
                }
            }

            Result = filtered.ToList();

            if (PricingFilterBody.RegionalLicense)
                Result = IncludeRegional(Result.ToList());

            FilterModel_View filterView = new FilterModel_View();

            filterView.RegionalLicense = PricingFilterBody.RegionalLicense;
            filterView.MinGDP = PricingFilterBody.MinGDP;
            filterView.MaxGDP = PricingFilterBody.MaxGDP;
            filterView.IsPairedAndUnPaired = PricingFilterBody.IsPairedAndUnPaired;
            filterView.IsPaired = PricingFilterBody.IsPaired;
            filterView.IsUnPaired = PricingFilterBody.IsUnPaired;
            filterView.Band = PricingFilterBody.Band;
            filterView.IsSingle = PricingFilterBody.IsSingle;

            Result = PerformFiltering(Result.ToList(), filterView);



            AdjustmentModel_View model = new AdjustmentModel_View();
            model.IsIncludeAnnual = PricingFilterBody.IsIncludeAnnual;
            model.DiscountRate = PricingFilterBody.DiscountRate;
            model.ISIMF = PricingFilterBody.ISIMF;
            model.AdjustByInflationFactor = PricingFilterBody.AdjustByInflationFactor;
            model.AdjustByPPPFactor = PricingFilterBody.AdjustByPPPFactor;
            model.AnnualizePrice = PricingFilterBody.AnnualizePrice;
            model.Term = PricingFilterBody.Term;
            model.AdjustByGDPFactor = PricingFilterBody.AdjustByGDPFactor;
            model.UniqueAwards = PricingFilterBody.UniqueAwards;
            model.SumBand = PricingFilterBody.SumBand;
            Result = PerformAdjustments(Result.ToList(), model);


            Result = Result.Where(m => m.Price != double.PositiveInfinity).ToList();

            Result = PerformAveraging(Result.ToList(), PricingFilterBody.AverageAwards, PricingFilterBody.AverageSumPricesAndMHZ, PricingFilterBody.SumBand, PricingFilterBody.ISIMF);


            if (PricingFilterBody.AverageAwards || PricingFilterBody.AverageSumPricesAndMHZ)
            {
                foreach (AwardsFiltered item in Result)
                {
                    if (item.Regionalicense == true)
                    {
                        var Pop = await IScocioEconomicRepository.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                           PricingFilterBody.ISIMF);
                        decimal? population = Pop.Item2;
                        if (PricingFilterBody.ISIMF == false)
                        {
                            population = population == null ? null : population / 1000000;
                        }
                        item.Pop = Convert.ToDouble(population);
                    }
                }
            }

            ExcludeOtliers_View view = new ExcludeOtliers_View();
            view.HasPercentile = PricingFilterBody.HasPercentile;
            view.HasQuartile = PricingFilterBody.HasQuartile;
            view.HasRegression = PricingFilterBody.HasRegression;
            view.HasStandardDeviation = PricingFilterBody.HasStandardDeviation;
            view.AutoFiltering = PricingFilterBody.AutoFiltering;
            view.KValue = PricingFilterBody.KValue;
            view.UpperPercentile = PricingFilterBody.UpperPercentile;
            view.LowerPercentile = PricingFilterBody.LowerPercentile;
            view.StandardDeviationValue = PricingFilterBody.StandardDeviationValue;
            view.Regression = PricingFilterBody.Regression;
            view.ShowMarkers = PricingFilterBody.ShowMarkers;


            Result = Result.Where(m => m.Price != double.PositiveInfinity && m.Price != 0).ToList();

            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
            && (m.Price != double.PositiveInfinity)).ToList();

            Result = PerfomPricing(Result.ToList(), view, PricingFilterBody.AdjustByGDPFactor);

            if (PricingFilterBody.UniqueAwards == true)
            {
                foreach (AwardsFiltered AwardsFiltered in Result)
                {
                    if (AwardsFiltered.Regionalicense)
                        AwardsFiltered.Pop = AwardsFiltered.AwardPop;

                }
            }


            return Ok(Result);
        }

        [HttpPost]
        [Route("FilterValuations")]
        public async Task<IActionResult> FilterValuations(ValuationBody PricingFilterBody)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            if (!PricingFilterBody.IsDistance)
            {

                var Result = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
                PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor, PricingFilterBody.AdjustByInflationFactor, PricingFilterBody.CountryIds, UserId);

                Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
                && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
                && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();


                List<AwardsFiltered> filtered = new List<AwardsFiltered>();
                foreach (AwardsFiltered item in Result)
                {
                    if (item.Pairing == "p")
                    {
                        if (item.BlockPaired != "" && item.BlockPaired != null)
                            filtered.Add(item);
                    }
                    else if (item.Pairing == "u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                            filtered.Add(item);
                    }
                    else if (item.Pairing == "p+u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                            item.BlockPaired != "")
                            filtered.Add(item);
                    }
                }

                Result = filtered.ToList();

                if (PricingFilterBody.RegionalLicense)
                    Result = IncludeRegional(Result.ToList());


                FilterModel_View filterView = new FilterModel_View();

                filterView.RegionalLicense = PricingFilterBody.RegionalLicense;
                filterView.MinGDP = PricingFilterBody.MinGDP;
                filterView.MaxGDP = PricingFilterBody.MaxGDP;
                filterView.IsPairedAndUnPaired = PricingFilterBody.IsPairedAndUnPaired;
                filterView.IsPaired = PricingFilterBody.IsPaired;
                filterView.IsUnPaired = PricingFilterBody.IsUnPaired;
                filterView.Band = PricingFilterBody.Band;
                filterView.IsSingle = PricingFilterBody.IsSingle;

                Result = PerformFiltering(Result.ToList(), filterView);

                if (PricingFilterBody.IsIncludeAnnual)
                {
                    foreach (AwardsFiltered item in Result)
                    {
                        try
                        {
                            double TotalPriceP = 0;
                            //if (item.AnnualFees != null)
                            //{

                            double _Term = Convert.ToDouble(item.Terms);

                            int Term = Convert.ToInt32(_Term);
                            double Summation = 0;
                            if (Term > 0)
                            {
                                for (int i = 1; i <= Term; i++)
                                {
                                    Summation += 1 / Math.Pow((1.0 + PricingFilterBody.DiscountRate / 100), i);

                                }
                            }

                            item.AnnualFees = item.AnnualFees == null ? 0 : item.AnnualFees;
                            double NetPresent = (double)(item.AnnualFees * Summation);
                            if (item.AuctionDateMonth.HasValue && item.AuctionDateYear.HasValue)
                            {

                                DateTime StartDate = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);
                                DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                                TimeSpan DelaySpan = StartDate - Auctiondt;
                                double YearsOfDelay = DelaySpan.TotalDays;
                                if (YearsOfDelay > 365)
                                {
                                    int TotalYears = (int)(YearsOfDelay / 365);
                                    double Mod = YearsOfDelay % 365;
                                    //double YearsOfDelayFractioned = TotalYears + (Mod / 365);
                                    double YearsOfDelayFractioned = item.AuctionDateYear.Value + ((double)item.AuctionDateMonth.Value / 12) - item.Year.Value - ((double)item.Month.Value / 12);
                                    double TotalAnnualPayment = NetPresent * Math.Pow(1 + PricingFilterBody.DiscountRate / 100, -(YearsOfDelayFractioned));
                                    double TotalPriceAtAuctionDate = (double)(item.UpFrontFees + TotalAnnualPayment);
                                    TotalPriceP = TotalPriceAtAuctionDate * Math.Pow(1 + PricingFilterBody.DiscountRate / 100, YearsOfDelayFractioned);
                                }
                                else
                                {
                                    TotalPriceP = (double)(item.UpFrontFees + NetPresent);

                                }
                            }
                            else
                            {

                                TotalPriceP = (double)(item.UpFrontFees + NetPresent);
                            }
                            item.Price = TotalPriceP;
                            item.PriceM = TotalPriceP;
                        }
                        catch (Exception e)
                        {

                        }

                    }
                }
                else
                {
                    foreach (AwardsFiltered item in Result)
                    {
                        double? TotalPriceP = 0;
                        if (item.AuctionDateYear.HasValue && item.AuctionDateMonth.HasValue)
                        {
                            DateTime StartDate = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);
                            DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                            TimeSpan DelaySpan = StartDate - Auctiondt;
                            double YearsOfDelay = DelaySpan.TotalDays;
                            if (YearsOfDelay > 365)
                            {
                                int TotalYears = (int)(YearsOfDelay / 365);
                                double Mod = YearsOfDelay % 365;


                                //double YearsOfDelayFractioned = TotalYears + (Mod / 365);
                                double YearsOfDelayFractioned = item.AuctionDateYear.Value + ((double)item.AuctionDateMonth.Value / 12) - item.Year.Value - ((double)item.Month.Value / 12);
                                TotalPriceP = (double)(item.UpFrontFees * Math.Pow(1 + PricingFilterBody.DiscountRate / 100, YearsOfDelayFractioned));
                            }
                            else
                            {
                                TotalPriceP = (double)(item.UpFrontFees);
                            }
                        }
                        else
                        {
                            TotalPriceP = item.UpFrontFees == null ? null : (double)item.UpFrontFees;
                        }
                        item.Price = TotalPriceP;
                        item.PriceM = TotalPriceP;
                    }

                }

                List<Band_TermView> userBandTerms = PricingFilterBody.BandTerms;
                foreach (AwardsFiltered item in Result)
                {
                    item.bandCountry = item.CountryName + "-" + item.Band + "(" + item.Year + ")";
                    if (PricingFilterBody.ISIMF == true)
                    {
                       // item.GDP = item.GDP * 1000;
                        item.AwardPop = item.AwardPop / 1000000;

                    }
                    if (PricingFilterBody.ISIMF == false)
                    {
                        item.Pop = item.Pop / 1000000;
                        item.AwardPop = item.AwardPop / 1000000;

                    }
                    if (item.Regionalicense)
                    {
                        if (item.AwardPop != 0)
                            item.Price = item.Price / item.AwardPop;
                    }
                    else
                    {
                        if (item.Pop != 0)
                            item.Price = item.Price / item.Pop;
                    }
                    string ItemBand = item.Band;

                    Band_TermView _term = userBandTerms.Where(x => x.Band.ToString() == ItemBand).FirstOrDefault();
                    double _Term = Convert.ToDouble(_term.Term);
                    int Term = Convert.ToInt32(_Term);
                    //int Term = Convert.ToInt32(_term.Term);
                    double LicenseFactor = 0;
                    double Numerator = 0;
                    double Denominator = 0;
                    for (int t = 0; t <= (Term * 12) - 1; t++)
                    {
                        double Fraction = 1 / (1 + (PricingFilterBody.DiscountRate / 100));
                        double pow = Convert.ToDouble(t) / 12.0;
                        Numerator += Math.Pow(Fraction, pow);//(1 / (Math.Pow(1 + int.Parse(item.Terms), t / 12)));
                    }
                    var _LicenseTerm = Convert.ToDouble(item.Terms) * 12;
                    for (int t = 0; t <= Convert.ToInt32(_LicenseTerm) - 1; t++)
                    {
                        double Fraction = 1 / (1 + (PricingFilterBody.DiscountRate / 100));
                        double pow = Convert.ToDouble(t) / 12.0;

                        Denominator += Math.Pow(Fraction, pow);
                    }
                    LicenseFactor = Numerator / Denominator;
                    item.Price = item.Price * LicenseFactor;

                    if (PricingFilterBody.AdjustByInflationFactor)
                    {
                        item.Price = item.Price * item.InflationFactor;
                    }
                    if (PricingFilterBody.AdjustByPPPFactor)
                    {
                        item.Price = item.Price * item.PPPFactor;
                    }
                    if (PricingFilterBody.AnnualizePrice)
                    {
                        if (PricingFilterBody.DiscountRate == 0)
                            item.Price = item.Price / Convert.ToDouble(item.Terms);
                        else
                            item.Price = item.Price * (PricingFilterBody.DiscountRate / 100) * (1 / (1 - (1 / (Math.Pow((1 + PricingFilterBody.DiscountRate / 100), Term)))));
                    }
                    if (PricingFilterBody.AdjustByGDPFactor)
                    {
                        decimal GDPC = (decimal)(item.GDP);
                        if (GDPC != 0)
                            item.Price = item.Price / Decimal.ToDouble(GDPC);
                    }
                    if (PricingFilterBody.UniqueAwards)
                    {
                        double value = 0;
                        if (item.BandPaired != null && item.BandPaired != "")
                            value = Convert.ToDouble(item.BlockPaired);

                        if (item.BandUnPaired != null && item.BandUnPaired != "")
                            value = Convert.ToDouble(item.BlockUnPaired);


                        if (item.BandUnPaired != null && item.BandUnPaired != "" && item.BandPaired != null && item.BandPaired != "")
                        {
                            if (PricingFilterBody.SumBand == "p")
                                value = Convert.ToDouble(item.BlockPaired);

                            else if (PricingFilterBody.SumBand == "u")
                                value = Convert.ToDouble(item.BlockUnPaired);

                            else if (PricingFilterBody.SumBand == "pu")
                                value = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);

                        }
                        if (value == 0)
                            item.Price = 0;
                        else
                            item.Price = item.Price / value;
                    }



                    if (item.SingleOrMultiBand == 's' || item.SingleOrMultiBand == 'i' || item.SingleOrMultiBand == 'S' || item.SingleOrMultiBand == 'I')
                    {
                        if (item.BandPaired != "")
                        {
                            item.CalculatedMHZ = Convert.ToDouble(item.BlockPaired);
                        }
                        if (item.BandUnPaired != "")
                        {
                            item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired);
                        }
                        if (item.BandUnPaired != "" && item.BandPaired != "")
                        {
                            item.CalculatedMHZ = Convert.ToDouble(item.BlockPaired) + Convert.ToDouble(item.BlockUnPaired);
                        }
                    }
                    else
                    {
                        item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                    }


                }



                Result = Result.Where(m => m.Price != double.PositiveInfinity).ToList();

                Result = PerformAveraging(Result.ToList(), PricingFilterBody.AverageAwards, PricingFilterBody.AverageSumPricesAndMHZ, PricingFilterBody.SumBand, PricingFilterBody.ISIMF);


                if (PricingFilterBody.AverageAwards || PricingFilterBody.AverageSumPricesAndMHZ)
                {
                    foreach (AwardsFiltered item in Result)
                    {
                        if (item.Regionalicense == true)
                        {
                            var Pop = await IScocioEconomicRepository.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                               PricingFilterBody.ISIMF);
                            decimal? population = Pop.Item2;
                            if (PricingFilterBody.ISIMF == false)
                            {
                                population = population ==  null ? null : population / 1000000;
                            }
                            item.Pop = Convert.ToDouble(population);
                        }
                    }
                }

                Result = Result.Where(m => m.Price != double.PositiveInfinity).ToList();

                ExcludeOtliers_View view = new ExcludeOtliers_View();
                view.HasPercentile = PricingFilterBody.HasPercentile;
                view.HasQuartile = PricingFilterBody.HasQuartile;
                view.HasRegression = PricingFilterBody.HasRegression;
                view.HasStandardDeviation = PricingFilterBody.HasStandardDeviation;
                view.AutoFiltering = PricingFilterBody.AutoFiltering;
                view.KValue = PricingFilterBody.KValue;
                view.UpperPercentile = PricingFilterBody.UpperPercentile;
                view.LowerPercentile = PricingFilterBody.LowerPercentile;
                view.StandardDeviationValue = PricingFilterBody.StandardDeviationValue;
                view.Regression = PricingFilterBody.Regression;
                view.ShowMarkers = PricingFilterBody.ShowMarkers;

                Result = PerfomPricing(Result.ToList(), view, PricingFilterBody.AdjustByGDPFactor);

                Result = Result.Where(m => m.Price != double.PositiveInfinity && m.Price != 0).ToList();

                // this is the extra part added to assign regional pop to the one being displayed.
                if (PricingFilterBody.UniqueAwards == true)
                {
                    foreach (AwardsFiltered AwardsFiltered in Result)
                    {
                        if (AwardsFiltered.Regionalicense)
                            AwardsFiltered.Pop = AwardsFiltered.AwardPop;

                    }
                }

                if (PricingFilterBody.IsBenchMark)
                    return Ok(Result.ToArray());
                else
                {
                    Result = Result.Where(m => m.Price != null).ToList();
                    Result = Result.Where(m => m.IsHidden == false).ToList();
                    List<XYPoint2> points = new List<XYPoint2>();
                    foreach (AwardsFiltered PricingFiltered in Result)
                    {
                        if (PricingFiltered.GDP != null)
                            points.Add(new XYPoint2() { X = (PricingFiltered.GDP.Value), Y = (double)PricingFiltered.Price, Id = PricingFiltered.Id, Price = (double)PricingFiltered.Price });
                    }
                    double a, b;
                    string Way;
                    if (points != null && points.Count > 0)
                    {
                        List<XYPoint2> bestFit = GenerateLinearBestFit2(points, PricingFilterBody.EnforeBPositive, out a, out b, out Way);

                        var GDP = await IScocioEconomicRepository.GetGDPForValuations(PricingFilterBody.IssueDate, Convert.ToInt32(PricingFilterBody.CountryId),
                            PricingFilterBody.IsPPP, PricingFilterBody.ISIMF);

                        var POP = await IScocioEconomicRepository.GetPopForValuations(PricingFilterBody.IssueDate, Convert.ToInt32(PricingFilterBody.CountryId),
                           PricingFilterBody.ISIMF);

                        double? _Pop = POP.Item2 == null ? null : (double)POP.Item2;
                        if (PricingFilterBody.ISIMF == false && _Pop != null)
                        {
                            _Pop = _Pop / 1000000;
                        }

                        double _FianlGDP = PricingFilterBody.ISIMF == true ? (double) GDP.Item2 * 1000 : (double)GDP.Item2;
                        Country country = await ICountryRepository.GetById(Convert.ToInt32(PricingFilterBody.CountryId));
                        AwardsFiltered predicted = new AwardsFiltered();
                        predicted.Id = 0;
                        predicted.CountryId = Convert.ToInt32(PricingFilterBody.CountryId);
                        predicted.Pop = PricingFilterBody.PopCovered == null ? _Pop : PricingFilterBody.PopCovered;
                        predicted.Year = PricingFilterBody.IssueDate;
                        predicted.Price = (double)(_FianlGDP * (double)a) + (double)b;
                        predicted.GDP = (decimal)_FianlGDP;
                        predicted.CountryName = PricingFilterBody.Lang  == "ar" ? country.NameAr : country.NameEn;
                        predicted.CountryId = Convert.ToInt32(PricingFilterBody.CountryId);
                        predicted.OperatorName = "";
                        predicted.Month = 0;
                        predicted.AuctionDateMonth = 0;
                        predicted.AuctionDateYear = 0;
                        predicted.AnnualFees = 0;
                        predicted.UpFrontFees = 0;
                        predicted.ReservePrice = 0;
                        predicted.Terms = PricingFilterBody.Terms.Replace(",", "/ ");
                        predicted.Band = PricingFilterBody.Band.Replace(",", "/ ");
                        predicted.Pairing = "";
                        predicted.MHZ = "";
                        predicted.Regionalicense = false;
                        predicted.Paid = 0;
                        predicted.coverage = "";
                        predicted.SingleOrMultiBand = 'S';
                        predicted.BandPaired = "";
                        predicted.BandUnPaired = "";
                        predicted.Id = 0;
                        predicted.InflationFactor = 0;
                        predicted.PPPFactor = 0;
                        predicted.GDPValue = 0;
                        predicted.BlockPaired = "";
                        predicted.BlockUnPaired = "";

                        predicted.CalculatedMHZ = 0;
                        predicted.bandCountry = "";

                        List<AwardsFiltered> lst = new List<AwardsFiltered>();
                        lst = Result.ToList();

                        lst.Insert(0, predicted);

                        foreach (AwardsFiltered item in lst)
                        {
                            item.aValue = (double)a;
                            item.bValue = (double)b;
                        }

                        return Ok(lst);

                    }

                }
                Result = Result.Where(m => m.Price != double.PositiveInfinity && m.Price != 0).ToList();
                return Ok(Result.ToList());
            }
            else
            {

                var Result = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
                PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor, PricingFilterBody.AdjustByInflationFactor,
                PricingFilterBody.CountryId, UserId);


                Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
                  && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
                  && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();

                List<AwardsFiltered> filtered = new List<AwardsFiltered>();
                foreach (AwardsFiltered item in Result)
                {
                    if (item.Pairing == "p")
                    {
                        if (item.BlockPaired != "" && item.BlockPaired != null)
                            filtered.Add(item);
                    }
                    else if (item.Pairing == "u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                            filtered.Add(item);
                    }
                    else if (item.Pairing == "p+u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                            item.BlockPaired != "")
                            filtered.Add(item);
                    }
                }

                Result = filtered.ToList();
                if (PricingFilterBody.RegionalLicense)
                    Result = IncludeRegional(Result.ToList());

                if (PricingFilterBody.MinGDP >= 0 && PricingFilterBody.MaxGDP >= 0)
                    Result = Result.Where(m => (m.GDP == null || m.GDP >= PricingFilterBody.MinGDP) && (m.GDP == null || m.GDP <= PricingFilterBody.MaxGDP)).ToList();

                if (PricingFilterBody.RegionalLicense == false)
                    Result = Result.Where(m => m.Regionalicense == false).ToList();

                var CopyResult = Result;
                var lstDistanceFilter = Result;

                if (PricingFilterBody.IsSingle == true)
                {
                    Result = Result.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i').ToList();
                }

                var SelectedAwards = new List<AwardsFiltered>();
                foreach (AwardsFiltered award in Result)
                {
                    if (award.Terms != null && award.UpFrontFees != null)
                    {
                        if (award.BandPaired != "" && award.BandUnPaired != "")
                        {
                            if (award.BlockPaired != null && award.BlockUnPaired != null)
                            {
                                SelectedAwards.Add(award);
                            }
                        }
                        else if (award.BandPaired != "")
                        {
                            if (award.BlockPaired != null)
                            {
                                SelectedAwards.Add(award);
                            }
                        }
                        else if (award.BandUnPaired != "")
                        {
                            if (award.BlockUnPaired != null)
                            {
                                SelectedAwards.Add(award);
                            }
                        }
                    }

                }

                var HighBand = new List<AwardsFiltered>();
                var LowBand = new List<AwardsFiltered>();
                var HighBandValues = new List<string>();
                var LowBandValues = new List<string>();


                foreach (AwardsFiltered award in SelectedAwards)
                {
                    if (award.BandPaired != null && award.BandPaired != "")
                    {
                        if (Int16.Parse(award.BandPaired) > Int16.Parse(PricingFilterBody.Band))
                        {
                            HighBand.Add(award);
                            if (!HighBandValues.Contains(award.BandPaired))
                            {
                                HighBandValues.Add(award.BandPaired);
                            }
                        }
                        else if (Int16.Parse(award.BandPaired) < Int16.Parse(PricingFilterBody.Band))
                        {
                            LowBand.Add(award);
                            if (!LowBandValues.Contains(award.BandPaired))
                            {
                                LowBandValues.Add(award.BandPaired);
                            }
                        }
                    }
                    else
                    {
                        if (award.BandUnPaired != null && award.BandUnPaired != "")
                        {
                            if (Int16.Parse(award.BandUnPaired) > Int16.Parse(PricingFilterBody.Band))
                            {
                                HighBand.Add(award);
                                if (!HighBandValues.Contains(award.BandUnPaired))
                                {
                                    HighBandValues.Add(award.BandUnPaired);
                                }
                            }
                            else if (Int16.Parse(award.BandUnPaired) < Int16.Parse(PricingFilterBody.Band))
                            {

                                LowBand.Add(award);
                                if (!LowBandValues.Contains(award.BandUnPaired))
                                {
                                    LowBandValues.Add(award.BandUnPaired);
                                }
                            }
                        }
                    }
                }
                if (LowBand.Count == 0 || HighBand.Count == 0)
                    return Ok("This method can't be applied since valuated country doesn't have enough bands");

                List<AwardsFiltered> FinalResult = new List<AwardsFiltered>();

                FinalResult.AddRange(LowBand);
                FinalResult.AddRange(HighBand);

                var ArrayListOfBandsInCountry = new List<List<AwardsFiltered>>();

                var ListHighBandWithCountries = new List<CustomAwardFiltered>();
                var ListLowBandWithCountries = new List<CustomAwardFiltered>();
                var TargetBandList = new CustomAwardFiltered();

                var res = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
                PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor,
                PricingFilterBody.AdjustByInflationFactor, PricingFilterBody.CountryIds, UserId);


                res = res.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
                   && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
                   && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();

                res = res.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i').ToList();

                List<AwardsFiltered> filtered2 = new List<AwardsFiltered>();
                foreach (AwardsFiltered item in res)
                {
                    if (item.Pairing == "p")
                    {
                        if (item.BlockPaired != "" && item.BlockPaired != null)
                            filtered2.Add(item);
                    }
                    else if (item.Pairing == "u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                            filtered2.Add(item);
                    }
                    else if (item.Pairing == "p+u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                            item.BlockPaired != "")
                            filtered2.Add(item);
                    }
                }

                res = filtered2.ToList();


                FilterModel_View filterView = new FilterModel_View();

                filterView.RegionalLicense = PricingFilterBody.RegionalLicense;
                filterView.MinGDP = PricingFilterBody.MinGDP;
                filterView.MaxGDP = PricingFilterBody.MaxGDP;
                filterView.IsPairedAndUnPaired = PricingFilterBody.IsPairedAndUnPaired;
                filterView.IsPaired = PricingFilterBody.IsPaired;
                filterView.IsUnPaired = PricingFilterBody.IsUnPaired;
                filterView.Band = PricingFilterBody.Band;
                filterView.IsSingle = PricingFilterBody.IsSingle;


                if (filterView.RegionalLicense == false)
                    res = res.Where(m => m.Regionalicense == false).ToList();
                if (filterView.MinGDP >= 0 && filterView.MaxGDP >= 0)
                    res = res.Where(m => (m.GDP >= filterView.MinGDP) && (m.GDP <= filterView.MaxGDP)).ToList();

                //if (!filterView.IsPairedAndUnPaired)
                //    res = res.Where(m => m.Pairing != "b").ToList();
                //if (!filterView.IsPaired)
                //    res = res.Where(m => m.Pairing != "p").ToList();
                //if (!filterView.IsUnPaired)
                //    res = res.Where(m => m.Pairing != "u").ToList();

                if (PricingFilterBody.RegionalLicense)
                    res = IncludeRegional(res.ToList());


                if (PricingFilterBody.RegionalLicense == false)
                    res = res.Where(m => m.Regionalicense == false).ToList();

                List<int?> CountryIds = new List<int?>();
                foreach (AwardsFiltered item in res)
                {
                    if (!CountryIds.Contains(item.CountryId))
                        CountryIds.Add(item.CountryId);
                }

                bool existOtherCountires = false;
                foreach (var i in CountryIds)
                {
                    if (i != Convert.ToInt32(PricingFilterBody.CountryId))
                    {
                        var tmpResult = res.Where(m => m.CountryId == i).ToList();

                        List<AwardsFiltered> HighBands = new List<AwardsFiltered>();
                        List<AwardsFiltered> LowBands = new List<AwardsFiltered>();
                        List<AwardsFiltered> TargetBands = new List<AwardsFiltered>();

                        foreach (string band in HighBandValues)
                        {
                            var CurrentBand = band;
                            var CustomAwardFiltered = new CustomAwardFiltered();
                            CustomAwardFiltered.Band = band;

                            List<AwardsFiltered> copyresFinal = tmpResult;
                            copyresFinal = copyresFinal.Where(m => m.BandPaired == band || m.BandUnPaired == band).ToList();
                            HighBands.AddRange(copyresFinal);
                        }

                        foreach (string band in LowBandValues)
                        {
                            var CurrentBand = band;
                            var CustomAwardFiltered = new CustomAwardFiltered();
                            CustomAwardFiltered.Band = band;
                            // need to get all awards of the same band in the countries

                            List<AwardsFiltered> copyresFinal = tmpResult;
                            copyresFinal = copyresFinal.Where(m => m.BandPaired == band || m.BandUnPaired == band).ToList();
                            //CustomAwardFiltered.Awards = copyresFinal.ToList();
                            LowBands.AddRange(copyresFinal);
                        }

                        foreach (AwardsFiltered aw in tmpResult)
                        {
                            if (PricingFilterBody.IsPaired)
                            {
                                if (aw.BandPaired == PricingFilterBody.Band && (aw.BandUnPaired == null || aw.BandUnPaired == ""))
                                    TargetBands.Add(aw);
                            }
                            if (PricingFilterBody.IsUnPaired)
                            {
                                if (aw.BandUnPaired == PricingFilterBody.Band && (aw.BandPaired == null || aw.BandPaired == ""))
                                    TargetBands.Add(aw);
                            }
                            if (PricingFilterBody.IsPairedAndUnPaired)
                            {
                                if (aw.BandPaired == PricingFilterBody.Band && aw.BandUnPaired == PricingFilterBody.Band)
                                    TargetBands.Add(aw);
                            }
                        }

                        if (HighBands.Count() > 0 && LowBands.Count() > 0 && TargetBands.Count() > 0)
                        {
                            FinalResult.AddRange(HighBands);
                            FinalResult.AddRange(LowBands);
                            FinalResult.AddRange(TargetBands);

                            existOtherCountires = true;
                        }
                    }

                }

                if (existOtherCountires == false)
                    return Ok("This method can't be applied since there are no countries with high, low and target bands");



                if (PricingFilterBody.IsIncludeAnnual)
                {
                    foreach (AwardsFiltered item in FinalResult)
                    {
                        try
                        {
                            double TotalPriceP = 0;
                            //if (item.AnnualFees != null)
                            //{
                            var _Term = Convert.ToDecimal(item.Terms);
                            int Term = Convert.ToInt32(_Term);

                            double Summation = 0;
                            if (Term > 0)
                            {
                                for (int i = 1; i <= Term; i++)
                                {
                                    Summation += 1 / Math.Pow((1.0 + PricingFilterBody.DiscountRate / 100), i);

                                }
                            }

                            item.AnnualFees = item.AnnualFees == null ? 0 : item.AnnualFees;
                            double NetPresent = (double)(item.AnnualFees * Summation);
                            if (item.AuctionDateMonth.HasValue && item.AuctionDateYear.HasValue)
                            {

                                DateTime StartDate = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);
                                DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                                TimeSpan DelaySpan = StartDate - Auctiondt;
                                double YearsOfDelay = DelaySpan.TotalDays;
                                if (YearsOfDelay > 365)
                                {
                                    int TotalYears = (int)(YearsOfDelay / 365);
                                    double Mod = YearsOfDelay % 365;
                                    //double YearsOfDelayFractioned = TotalYears + (Mod / 365);
                                    double YearsOfDelayFractioned = item.AuctionDateYear.Value + ((double)item.AuctionDateMonth.Value / 12) - item.Year.Value - ((double)item.Month.Value / 12);
                                    double TotalAnnualPayment = NetPresent * Math.Pow(1 + PricingFilterBody.DiscountRate / 100, -(YearsOfDelayFractioned));
                                    double TotalPriceAtAuctionDate = (double)(item.UpFrontFees + TotalAnnualPayment);
                                    TotalPriceP = TotalPriceAtAuctionDate * Math.Pow(1 + PricingFilterBody.DiscountRate / 100, YearsOfDelayFractioned);
                                }
                                else
                                {
                                    TotalPriceP = (double)(item.UpFrontFees + NetPresent);

                                }
                            }
                            else
                            {

                                TotalPriceP = (double)(item.UpFrontFees + NetPresent);
                            }
                            item.Price = TotalPriceP;
                            item.PriceM = TotalPriceP;
                        }
                        catch(Exception e)
                        {

                        }
              
                    }
                }
                else
                {
                    foreach (AwardsFiltered item in FinalResult)
                    {
                        try
                        {
                            double? TotalPriceP = 0;
                            if (item.AuctionDateYear.HasValue && item.AuctionDateMonth.HasValue)
                            {
                                DateTime StartDate = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);
                                DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                                TimeSpan DelaySpan = StartDate - Auctiondt;
                                double YearsOfDelay = DelaySpan.TotalDays;
                                if (YearsOfDelay > 365)
                                {
                                    int TotalYears = (int)(YearsOfDelay / 365);
                                    double Mod = YearsOfDelay % 365;


                                    //   double YearsOfDelayFractioned = TotalYears + (Mod / 365);
                                    double YearsOfDelayFractioned = item.AuctionDateYear.Value + ((double)item.AuctionDateMonth.Value / 12) - item.Year.Value - ((double)item.Month.Value / 12);
                                    TotalPriceP = (double)(item.UpFrontFees * Math.Pow(1 + PricingFilterBody.DiscountRate / 100, YearsOfDelayFractioned));
                                }
                                else
                                {
                                    TotalPriceP = (double)(item.UpFrontFees);
                                }
                            }
                            else
                            {
                                TotalPriceP = item.UpFrontFees == null ? null : (double)item.UpFrontFees;
                            }
                            item.Price = TotalPriceP;
                            item.PriceM = TotalPriceP;
                        }
                        catch(Exception e)
                        {

                        }
                     
                    }
                }

                List<Band_TermView> userBandTerms = PricingFilterBody.BandTerms;
                foreach (AwardsFiltered item in FinalResult)
                {
                    try
                    {

                        item.bandCountry = item.CountryName + "-" + item.Band + "(" + item.Year + ")";
                        if (PricingFilterBody.ISIMF == true)
                        {
                            //  item.GDP = item.GDP * 1000;
                            item.AwardPop = item.AwardPop / 1000000;

                        }
                        if (PricingFilterBody.ISIMF == false)
                        {
                            item.Pop = item.Pop / 1000000;
                            item.AwardPop = item.AwardPop / 1000000;

                        }
                        if (item.Regionalicense)
                        {
                            if (item.AwardPop != 0)
                                item.Price = item.Price / item.AwardPop;
                        }
                        else
                        {
                            if (item.Pop != 0)
                                item.Price = item.Price / item.Pop;
                        }
                        string ItemBand = item.Band;


                        Band_TermView _term = userBandTerms.Where(x => x.Band.ToString() == PricingFilterBody.Band).FirstOrDefault();
                        int Term = Convert.ToInt32(_term.Term);
                        double LicenseFactor = 0;
                        double Numerator = 0;
                        double Denominator = 0;
                        for (int t = 0; t <= (Term * 12) - 1; t++)
                        {
                            double Fraction = 1 / (1 + (PricingFilterBody.DiscountRate / 100));
                            double pow = Convert.ToDouble(t) / 12.0;
                            Numerator += Math.Pow(Fraction, pow);//(1 / (Math.Pow(1 + int.Parse(item.Terms), t / 12)));
                        }
                        var _LicenseTerm = Convert.ToDouble(item.Terms) * 12;
                        for (int t = 0; t <= Convert.ToInt32(_LicenseTerm) - 1; t++)
                        {
                            double Fraction = 1 / (1 + (PricingFilterBody.DiscountRate / 100));
                            double pow = Convert.ToDouble(t) / 12.0;

                            Denominator += Math.Pow(Fraction, pow);
                        }
                        //Ask Faten
                        LicenseFactor = Numerator / Denominator;
                        item.Price = item.Price * LicenseFactor;

                        if (PricingFilterBody.AdjustByInflationFactor)
                        {
                            item.Price = item.Price * item.InflationFactor;
                        }
                        if (PricingFilterBody.AdjustByPPPFactor)
                        {
                            item.Price = item.Price * item.PPPFactor;
                        }
                        if (PricingFilterBody.AnnualizePrice)
                        {
                            if (PricingFilterBody.DiscountRate == 0)
                                item.Price = item.Price / Convert.ToDouble(item.Terms);
                            else
                                item.Price = item.Price * (PricingFilterBody.DiscountRate / 100) * (1 / (1 - (1 / (Math.Pow((1 + PricingFilterBody.DiscountRate / 100), Term)))));

                        }
                        if (PricingFilterBody.AdjustByGDPFactor)
                        {
                            decimal GDPC = (decimal)(item.GDP);
                            if (GDPC != 0)
                                item.Price = item.Price / Decimal.ToDouble(GDPC);
                        }


                        if (item.SingleOrMultiBand == 's' || item.SingleOrMultiBand == 'i' || item.SingleOrMultiBand == 'S' || item.SingleOrMultiBand == 'I')
                        {
                            if (item.BandPaired != "")
                            {
                                item.CalculatedMHZ = Convert.ToDouble(item.BlockPaired);
                            }
                            if (item.BandUnPaired != "")
                            {
                                item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired);
                            }
                            if (item.BandUnPaired != "" && item.BandPaired != "")
                            {
                                item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired + item.BlockPaired);
                            }
                        }
                        else
                        {
                            item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                        }

                    }
                    catch (Exception e)
                    {

                    }


                }

                FinalResult = FinalResult.Where(m => m.Price != double.PositiveInfinity).ToList();
                FinalResult = FinalResult.ToList();
                bool isAveraging = (PricingFilterBody.AverageAwards || PricingFilterBody.UniqueAwards);
                FinalResult = PerformAveraging(FinalResult.ToList(), isAveraging, PricingFilterBody.AverageSumPricesAndMHZ, PricingFilterBody.SumBand, PricingFilterBody.ISIMF);


                if (isAveraging || PricingFilterBody.AverageSumPricesAndMHZ)
                {
                    foreach (AwardsFiltered item in Result)
                    {
                        if (item.Regionalicense == true)
                        {
                            var Pop = await IScocioEconomicRepository.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                               PricingFilterBody.ISIMF);
                            decimal? population = Pop.Item2;
                            if (PricingFilterBody.ISIMF == false)
                            {
                                population = population == null ? null : population / 1000000;
                            }
                            item.Pop = Convert.ToDouble(population);
                        }
                    }
                }

                FinalResult = FinalResult.Where(m => m.Price != double.PositiveInfinity).ToList();

                ExcludeOtliers_View view = new ExcludeOtliers_View();
                view.HasPercentile = PricingFilterBody.HasPercentile;
                view.HasQuartile = PricingFilterBody.HasQuartile;
                view.HasRegression = PricingFilterBody.HasRegression;
                view.HasStandardDeviation = PricingFilterBody.HasStandardDeviation;
                view.AutoFiltering = PricingFilterBody.AutoFiltering;
                view.KValue = PricingFilterBody.KValue;
                view.UpperPercentile = PricingFilterBody.UpperPercentile;
                view.LowerPercentile = PricingFilterBody.LowerPercentile;
                view.StandardDeviationValue = PricingFilterBody.StandardDeviationValue;
                view.Regression = PricingFilterBody.Regression;
                view.ShowMarkers = PricingFilterBody.ShowMarkers;

                FinalResult = PerfomPricing(FinalResult.ToList(), view, PricingFilterBody.AdjustByGDPFactor);

                FinalResult = FinalResult.Where(m => m.Price != double.PositiveInfinity && m.Price != 0).ToList();
                FinalResult = FinalResult.Where(m => m.Price != double.PositiveInfinity).ToList();
                //Done Pricing
                return Ok(FinalResult);
            }
        }

        [HttpPost]
        [Route("GetDistancingResult")]
        public async Task<IActionResult> PerformPermutation(Distancing_PermutationView view)
        {
            List<AwardsFiltered> tmpList = new List<AwardsFiltered>();
            List<AwardsFiltered> List = view.FilteredAwards;
            List<AwardsFiltered> LowBands = List.Where(m => Convert.ToInt32(m.Band) < Convert.ToInt32(view.TargetBand)).ToList();
            List<AwardsFiltered> TargetBands = List.Where(m => Convert.ToInt32(m.Band) == Convert.ToInt32(view.TargetBand)).ToList();
            List<AwardsFiltered> HighBands = List.Where(m => Convert.ToInt32(m.Band) > Convert.ToInt32(view.TargetBand)).ToList();

            List<AwardsFiltered> ValuatedCountryBands = List.Where(m => m.CountryId == view.ValutatedCountryId).ToList();

            List<Distancing_View> distancingView = new List<Distancing_View>();

            List = List.Where(m => m.CountryId != view.ValutatedCountryId).ToList();

            foreach (AwardsFiltered item in List)
            {
                if (distancingView.Where(m => m.CountryId == item.CountryId).ToList().Count() == 0)
                {
                    tmpList = List.Where(m => m.CountryId == item.CountryId).ToList();
                    List<AwardsFiltered> tmpLowBands = LowBands.Where(m => m.CountryId == item.CountryId).ToList();
                    List<AwardsFiltered> tmpHighBands = HighBands.Where(m => m.CountryId == item.CountryId).ToList();
                    AwardsFiltered tmpTaretBands = TargetBands.Where(m => m.CountryId == item.CountryId).OrderByDescending(m => m.Year).FirstOrDefault();

                    foreach (AwardsFiltered countryLowBand in tmpLowBands)
                    {
                       
                        var lowVband = ValuatedCountryBands.Where(m => m.Band == countryLowBand.Band).OrderByDescending(m => m.Year).FirstOrDefault();
                       
                        foreach (AwardsFiltered highBands in tmpHighBands)
                        {
                            Distancing_View tmpResult = new Distancing_View();

                            tmpResult.LowBand = countryLowBand.Band;
                            tmpResult.CountryName = countryLowBand.CountryName;
                            tmpResult.CountryId = countryLowBand.CountryId;
                            tmpResult.LowBandYear = countryLowBand.Year;
                            tmpResult.LowBandPrice = countryLowBand.Price;

                            tmpResult.TargetBand = tmpTaretBands.Band;
                            tmpResult.TargetBandYear = tmpTaretBands.Year;
                            tmpResult.TargetBandPrice = tmpTaretBands.Price;

                            tmpResult.ValuatedLowBand = lowVband.Band;
                            tmpResult.ValuatedLowBandPrice = lowVband.Price;
                            tmpResult.ValuatedLowBandYear = lowVband.Year;

                            var highVband = ValuatedCountryBands.Where(m => m.Band == highBands.Band).OrderByDescending(m => m.Year).FirstOrDefault();
                            tmpResult.HighBand = highBands.Band;
                            tmpResult.HighBandYear = highBands.Year;
                            tmpResult.HighBandPrice = highBands.Price;

                            tmpResult.ValuatedHighBand = highVband.Band;
                            tmpResult.ValuatedHighBandPrice = highVband.Price;
                            tmpResult.ValuatedHighBandYear = highVband.Year;


                            tmpResult.RelativePrice = (tmpResult.TargetBandPrice - tmpResult.HighBandPrice) / (tmpResult.LowBandPrice - tmpResult.HighBandPrice) * (tmpResult.ValuatedLowBandPrice - tmpResult.ValuatedHighBandPrice) + tmpResult.ValuatedHighBandPrice;

                            distancingView.Add(tmpResult);

                        }
                    }
                }

            }

            distancingView = distancingView.Where(m => m.RelativePrice > 0).ToList();

            return Ok(distancingView);
        }

        public static List<AwardsFiltered> PerformFiltering(List<AwardsFiltered> Result, FilterModel_View view)
        {
            if (view.RegionalLicense == false)
                Result = Result.Where(m => m.Regionalicense == false).ToList();
            if (view.MinGDP >= 0 && view.MaxGDP >= 0)
                Result = Result.Where(m => (m.GDP >= view.MinGDP) && (m.GDP <= view.MaxGDP)).ToList();

            if (!view.IsPairedAndUnPaired)
                Result = Result.Where(m => m.Pairing != "p+u").ToList();
            if (!view.IsPaired)
                Result = Result.Where(m => m.Pairing != "p").ToList();
            if (!view.IsUnPaired)
                Result = Result.Where(m => m.Pairing != "u").ToList();
            string[] SelectedBands = view.Band.Split(',');
            var FilteredResults = new List<AwardsFiltered>();
            if (view.IsSingle)
            {
                if (view.IsPaired)
                {
                    foreach (AwardsFiltered Award in Result)
                    {
                        if (SelectedBands.Contains(Award.BandPaired) && (Award.BandUnPaired == "" || Award.BandUnPaired == null))
                            FilteredResults.Add(Award);
                    }
                }
                if (view.IsUnPaired)
                {
                    foreach (AwardsFiltered Award in Result)
                    {
                        if (SelectedBands.Contains(Award.BandUnPaired) && (Award.BandPaired == "" || Award.BandPaired == null))
                            FilteredResults.Add(Award);
                    }
                }
                if (view.IsPairedAndUnPaired)
                {
                    foreach (AwardsFiltered Award in Result)
                    {
                        if (SelectedBands.Contains(Award.BandUnPaired) && SelectedBands.Contains(Award.BandPaired)
                            && Award.BandPaired != "" && Award.BandUnPaired != null && Award.BandPaired != null
                            && Award.BandUnPaired != "")
                            if (!FilteredResults.Contains(Award))
                                FilteredResults.Add(Award);
                    }
                }
                Result = FilteredResults;
            }


            if (view.IsSingle == true)
            {
                Result = Result.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i').ToList();
            }

            return Result;
        }

        public static List<AwardsFiltered> PerfomPricing(List<AwardsFiltered> AwardsFiltered, ExcludeOtliers_View view, bool AdjustbyGDPFactor = false)
        {
            try
            {
                if (AdjustbyGDPFactor == true)
                {
                    foreach (AwardsFiltered item in AwardsFiltered)
                    {
                        item.PriceForFilter = item.Price;
                    }
                }
                else
                {
                    foreach (AwardsFiltered item in AwardsFiltered)
                    {
                        decimal GDPC = (decimal)(item.GDP);
                        if (GDPC != 0)
                            item.PriceForFilter = item.Price / Decimal.ToDouble(GDPC);
                    }
                }

                if (view.HasPercentile == true)
                {
                    if (view.UpperPercentile != 100 || view.LowerPercentile != 0)
                    {
                        List<AwardsFiltered> SortedList = AwardsFiltered.OrderBy(o => o.PriceForFilter).ToList();
                        double MaxPosition = (double)((view.UpperPercentile / 100) * (double)AwardsFiltered.Count());
                        double MinPosition = (double)(view.LowerPercentile / 100) * (double)AwardsFiltered.Count();

                        int MinInd = Convert.ToInt32(Math.Floor(MinPosition));
                        int MaxInd = Convert.ToInt32(Math.Ceiling(MaxPosition));
                        var ValueAtMax = SortedList.ElementAt((MaxInd - 1));
                        var ValueAtMin = SortedList.ElementAt(MinInd == 0 ? MinInd : (MinInd - 1));

                        if (ValueAtMax.PriceForFilter != null && ValueAtMin.PriceForFilter != null)
                            AwardsFiltered = AwardsFiltered.Where(m => m.PriceForFilter > ValueAtMin.PriceForFilter && m.PriceForFilter <= ValueAtMax.PriceForFilter).ToList();

                        //if (view.ShowMarkers == false || view.ShowMarkers == null)
                        //{
                        //    if (ValueAtMax.PriceForFilter != null && ValueAtMin.PriceForFilter != null)
                        //        AwardsFiltered = AwardsFiltered.Where(m => m.PriceForFilter > ValueAtMin.PriceForFilter && m.PriceForFilter <= ValueAtMax.PriceForFilter).ToList();
                        //}
                        //else
                        //{
                        //    foreach (AwardsFiltered item in AwardsFiltered)
                        //    {
                        //        if (item.PriceForFilter > ValueAtMin.PriceForFilter && item.PriceForFilter <= ValueAtMax.PriceForFilter)
                        //        {
                        //            item.IsHidden = false;
                        //            item.UpperValue = ValueAtMax.PriceForFilter;
                        //            item.LowerValue = ValueAtMin.PriceForFilter;

                        //        }
                        //        else
                        //        {
                        //            item.IsHidden = true;
                        //            item.UpperValue = ValueAtMax.PriceForFilter;
                        //            item.LowerValue = ValueAtMin.PriceForFilter;
                        //        }
                        //    }
                        //}

                    }
                }
                if (view.HasQuartile == true)
                {
                    List<AwardsFiltered> SortedList = AwardsFiltered.OrderBy(o => o.PriceForFilter).ToList();
                    double MaxPosition = (double)(0.75 * AwardsFiltered.Count());
                    double MinPosition = (double)(0.25 * AwardsFiltered.Count());

                    double ValueAtMax = (double)SortedList.ElementAt(Convert.ToInt32(MaxPosition) - 1).PriceForFilter;
                    double ValueAtMin = (double)SortedList.ElementAt(Convert.ToInt32(MinPosition) == 0 ? 0 : Convert.ToInt32(MinPosition) - 1).PriceForFilter;

                    double IQR = ValueAtMax - ValueAtMin;
                    double? MinValue = ValueAtMin - (view.KValue * IQR);
                    double? MaxValue = ValueAtMax + (view.KValue * IQR);

                    AwardsFiltered = AwardsFiltered.Where(m => m.PriceForFilter > MinValue && m.PriceForFilter < MaxValue).ToList();

                    //if (view.ShowMarkers == false || view.ShowMarkers == null)
                    //{
                    //    AwardsFiltered = AwardsFiltered.Where(m => m.PriceForFilter > MinValue && m.PriceForFilter < MaxValue).ToList();
                    //}
                    //else
                    //{
                    //    foreach (AwardsFiltered item in AwardsFiltered)
                    //    {
                    //        if (item.PriceForFilter > MinValue && item.PriceForFilter < MaxValue)
                    //        {
                    //            item.IsHidden = false;
                    //            item.UpperValue = MaxValue;
                    //            item.LowerValue = MinValue;

                    //        }
                    //        else
                    //        {
                    //            item.IsHidden = true;
                    //            item.UpperValue = MaxValue;
                    //            item.LowerValue = MinValue;
                    //        }
                    //    }
                    //}
                }
                if (view.HasStandardDeviation == true)
                {
                    double Mean = 0;
                    double STD = 0;
                    double Sum = 0;
                    double Variance = 0;
                    double SumSQR = 0;
                    foreach (AwardsFiltered Award in AwardsFiltered)
                    {
                        Sum += (double)Award.PriceForFilter;

                    }
                    Mean = Sum / AwardsFiltered.Count();
                    foreach (AwardsFiltered Award in AwardsFiltered)
                    {

                        double deviation = (double)(Award.PriceForFilter - Mean);
                        SumSQR += deviation * deviation;
                    }

                    Variance = SumSQR / AwardsFiltered.Count();

                    STD = (float)Math.Sqrt(Variance);

                    foreach (AwardsFiltered Award in AwardsFiltered)
                    {
                        Award.UpperValue = view.StandardDeviationValue * STD + Mean;
                        Award.LowerValue = -view.StandardDeviationValue * STD + Mean;

                        if (Math.Abs((double)(Award.PriceForFilter - Mean)) > (view.StandardDeviationValue * STD))
                        {
                            AwardsFiltered = AwardsFiltered.Where(m => m.Id != Award.Id).ToList();
                            //if (view.ShowMarkers == false || view.ShowMarkers == null)
                            //    AwardsFiltered = AwardsFiltered.Where(m => m.Id != Award.Id).ToList();
                            //else
                            //{
                            //    Award.IsHidden = true;
                            //}

                        }
                    }
                }
                if (view.HasRegression == true)
                {
                    List<XYPoint2> points = new List<XYPoint2>();
                    foreach (AwardsFiltered PricingFiltered in AwardsFiltered)
                    {
                        if (PricingFiltered.GDP != null)
                            points.Add(new XYPoint2() { X = (PricingFiltered.GDP.Value), Y = (double)PricingFiltered.Price, Id = PricingFiltered.Id, Price = (double)PricingFiltered.Price });
                    }
                    double a, b;
                    if (points != null && points.Count > 0)
                    {
                        List<XYPoint2> bestFit = GenerateLinearBestFit(points, out a, out b);
                        double regValue = (double)view.Regression / (double)100;
                        double NumberOfPoints = (double)regValue * (double)AwardsFiltered.Count();
                        while ((int)NumberOfPoints > 0)
                        {
                            double Max = (double)Decimal.MinValue;
                            int PointToRemoveId = 0;

                            for (int index = 0; index < points.Count; index++)
                            {
                                //decimal MSE = (decimal)Math.Pow((double)bestFit[index].Price, 2);
                                double MSE = (double)Math.Pow(((double)bestFit[index].Price - bestFit[index].Y), 2);
                                if (Max < MSE)
                                {
                                    Max = MSE;
                                    PointToRemoveId = points[index].Id;
                                }
                            }
                            if (PointToRemoveId > 0)
                            {
                                foreach (XYPoint2 Point in points)
                                {
                                    if (Point.Id == PointToRemoveId)
                                    {
                                        points.Remove(Point);
                                        AwardsFiltered = AwardsFiltered.Where(m => m.Id != Point.Id).ToList();
                                        break;
                                    }
                                }
                                bestFit = GenerateLinearBestFit(points, out a, out b);
                                NumberOfPoints--;
                            }
                        }
                    }
                    // Console.WriteLine("y = {0:#.####}x {1:+#.####;-#.####}", a, -b);

                }
                if (view.AutoFiltering == true)
                {
                  List<AwardsFiltered> SortedList = AwardsFiltered.OrderBy(o => o.PriceForFilter).ToList();
                        double MaxPosition = (double)((0.75) * (double)AwardsFiltered.Count());
                        double MinPosition = (double)(0.25) * (double)AwardsFiltered.Count();

                    int MinInd = Convert.ToInt32(Math.Floor(MinPosition));
                    int MaxInd = Convert.ToInt32(Math.Ceiling(MaxPosition));
                    var ValueAtMax = SortedList.ElementAt((MaxInd - 1));
                    var ValueAtMin = SortedList.ElementAt(MinInd == 0 ? MinInd : (MinInd - 1));

                    if (ValueAtMax.PriceForFilter != null && ValueAtMin.PriceForFilter != null)
                        AwardsFiltered = AwardsFiltered.Where(m => m.PriceForFilter > ValueAtMin.PriceForFilter && m.PriceForFilter <= ValueAtMax.PriceForFilter).ToList();

                    //if (view.ShowMarkers == false || view.ShowMarkers == null)
                    //    {
                    //        if (ValueAtMax.PriceForFilter != null && ValueAtMin.PriceForFilter != null)
                    //            AwardsFiltered = AwardsFiltered.Where(m => m.PriceForFilter > ValueAtMin.PriceForFilter && m.PriceForFilter < ValueAtMax.PriceForFilter).ToList();
                    //    }
                    //    else
                    //    {
                    //        foreach (AwardsFiltered item in AwardsFiltered)
                    //        {
                    //            if (item.PriceForFilter > ValueAtMin.PriceForFilter && item.PriceForFilter < ValueAtMax.PriceForFilter)
                    //            {
                    //                item.IsHidden = false;
                    //                item.UpperValue = ValueAtMax.PriceForFilter;
                    //                item.LowerValue = ValueAtMin.PriceForFilter;

                    //            }
                    //            else
                    //            {
                    //                item.IsHidden = true;
                    //                item.UpperValue = ValueAtMax.PriceForFilter;
                    //                item.LowerValue = ValueAtMin.PriceForFilter;
                    //            }
                    //        }
                    //    }
                }
            }

            catch (Exception e)
            {

            }
            return AwardsFiltered;
        }

        public static List<AwardsFiltered> PerformAdjustments(List<AwardsFiltered> Result, AdjustmentModel_View model)
        {
            if (model.IsIncludeAnnual)
            {
                foreach (AwardsFiltered item in Result)
                {
                    try
                    {
                        double TotalPriceP = 0;

                        var _Term = Convert.ToDecimal(item.Terms);
                        int Term = Convert.ToInt32(_Term);
                        double Summation = 0;
                        if (Term > 0)
                        {
                            for (int i = 1; i <= Term; i++)
                            {
                                Summation += 1 / Math.Pow((1.0 + model.DiscountRate / 100), i);

                            }
                        }

                        item.AnnualFees = item.AnnualFees == null ? 0 : item.AnnualFees;
                        double NetPresent = (double)(item.AnnualFees * Summation);
                        if (item.AuctionDateMonth.HasValue && item.AuctionDateYear.HasValue)
                        {

                            DateTime StartDate = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);
                            DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                            TimeSpan DelaySpan = StartDate - Auctiondt;
                            double YearsOfDelay = DelaySpan.TotalDays;
                            if (YearsOfDelay > 365)
                            {
                                int TotalYears = (int)(YearsOfDelay / 365);
                                double Mod = YearsOfDelay % 365;
                                double YearsOfDelayFractioned = item.AuctionDateYear.Value + ((double)item.AuctionDateMonth.Value / 12) - item.Year.Value - ((double)item.Month.Value / 12);
                                double TotalAnnualPayment = NetPresent * Math.Pow(1 + model.DiscountRate / 100, -(YearsOfDelayFractioned));
                                double TotalPriceAtAuctionDate = (double)(item.UpFrontFees + TotalAnnualPayment);
                                TotalPriceP = TotalPriceAtAuctionDate * Math.Pow(1 + model.DiscountRate / 100, YearsOfDelayFractioned);
                            }
                            else
                            {
                                TotalPriceP = (double)(item.UpFrontFees + NetPresent);

                            }
                        }
                        else
                        {

                            TotalPriceP = (double)(item.UpFrontFees + NetPresent);
                        }
                        item.Price = TotalPriceP;
                        item.PriceM = TotalPriceP;
                    }
                    catch (Exception E)
                    {

                    }
                }
            }
            else
            {
                foreach (AwardsFiltered item in Result)
                {
                    double? TotalPriceP = 0;
                    if (item.AuctionDateYear.HasValue && item.AuctionDateMonth.HasValue)
                    {
                        DateTime StartDate = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);
                        DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                        TimeSpan DelaySpan = StartDate - Auctiondt;
                        double YearsOfDelay = DelaySpan.TotalDays;
                        if (YearsOfDelay > 365)
                        {
                            int TotalYears = (int)(YearsOfDelay / 365);
                            double Mod = YearsOfDelay % 365;

                            double YearsOfDelayFractioned = item.AuctionDateYear.Value + ((double)item.AuctionDateMonth.Value / 12) - item.Year.Value - ((double)item.Month.Value / 12);
                            TotalPriceP = (double)(item.UpFrontFees * Math.Pow(1 + model.DiscountRate / 100, YearsOfDelayFractioned));
                        }
                        else
                        {
                            TotalPriceP = (double)(item.UpFrontFees);
                        }
                    }
                    else
                    {
                        TotalPriceP = item.UpFrontFees == null ? null : (double)item.UpFrontFees;
                    }
                    item.Price = TotalPriceP;
                    item.PriceM = TotalPriceP;
                }

            }

            foreach (AwardsFiltered item in Result)
            {
                try
                {
                    item.bandCountry = item.CountryName + "-" + item.Band + "(" + item.Year + ")";

                    if (model.ISIMF == true)
                    {
                      //  item.GDP = item.GDP * 1000;
                        item.AwardPop = item.AwardPop / 1000000;

                    }

                    if (model.ISIMF == false)
                    {
                        item.Pop = item.Pop / 1000000;
                        item.AwardPop = item.AwardPop / 1000000;

                    }
                    if (item.Regionalicense)
                    {
                        if (item.AwardPop != 0)
                            item.Price = item.Price / item.AwardPop;
                    }
                    else
                    {
                        if (item.Pop != 0)
                            item.Price = item.Price / item.Pop;
                    }

                    double LicenseFactor = 0;
                    double Numerator = 0;
                    double Denominator = 0;
                    for (int t = 0; t <= (model.Term * 12) - 1; t++)
                    {
                        double Fraction = 1 / (1 + (model.DiscountRate / 100));
                        double pow = Convert.ToDouble(t) / 12.0;
                        Numerator += Math.Pow(Fraction, pow);//(1 / (Math.Pow(1 + int.Parse(item.Terms), t / 12)));
                    }
                    var _LicenseTerm = Convert.ToDouble(item.Terms) * 12;

                    for (int t = 0; t <= Convert.ToInt32(_LicenseTerm) - 1; t++)
                    {
                        double Fraction = 1 / (1 + (model.DiscountRate / 100));
                        double pow = Convert.ToDouble(t) / 12.0;

                        Denominator += Math.Pow(Fraction, pow);
                    }
                    LicenseFactor = Numerator / Denominator;
                    item.Price = item.Price * LicenseFactor;

                    if (model.AdjustByInflationFactor)
                    {
                        item.Price = item.Price * item.InflationFactor;
                    }
                    if (model.AdjustByPPPFactor)
                    {
                        item.Price = item.Price * item.PPPFactor;
                    }
                    if (model.AnnualizePrice)
                    {
                        if (model.DiscountRate == 0)
                            item.Price = item.Price / Convert.ToDouble(item.Terms);
                        else 
                            item.Price = item.Price * (model.DiscountRate / 100) * (1 / (1 - (1 / (Math.Pow((1 + model.DiscountRate / 100), model.Term)))));
                    }
                    if (model.AdjustByGDPFactor)
                    {
                        decimal GDPC = (decimal)(item.GDP);
                        if (GDPC != 0)
                            item.Price = item.Price / Decimal.ToDouble(GDPC);
                    }
                    if (model.UniqueAwards)
                    {
                        double value = 0;
                        if (item.BandPaired != null && item.BandPaired != "")
                            value = Convert.ToDouble(item.BlockPaired);

                        if (item.BandUnPaired != null && item.BandUnPaired != "")
                            value = Convert.ToDouble(item.BlockUnPaired);


                        if (item.BandUnPaired != null && item.BandUnPaired != "" && item.BandPaired != null && item.BandPaired != "")
                        {
                            if (model.SumBand == "p")
                                value = Convert.ToDouble(item.BlockPaired);

                            else if (model.SumBand == "u")
                                value = Convert.ToDouble(item.BlockUnPaired);

                            else if (model.SumBand == "pu")
                                value = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);

                        }
                        item.Price = item.Price / value;
                    }

                    if (item.SingleOrMultiBand == 's' || item.SingleOrMultiBand == 'i' || item.SingleOrMultiBand == 'S' || item.SingleOrMultiBand == 'I')
                    {
                        if (item.BandPaired != "")
                        {
                            item.CalculatedMHZ = Convert.ToDouble(item.BlockPaired);
                        }
                        if (item.BandUnPaired != "")
                        {
                            item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired);
                        }
                        if (item.BandUnPaired != "" && item.BandPaired != "")
                        {
                            var sum = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                            item.CalculatedMHZ = Convert.ToDouble(sum);
                        }
                    }
                    else
                    {
                        item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                    }

                }
                catch (Exception e)
                {

                }


            }

            return Result;
        }


        public static List<AwardsFiltered> PerformAveraging(List<AwardsFiltered> Result, bool AverageAwards, bool AverageSumPricesAndMHZ, string SumBand, bool IsIMF)
        {
            List<AwardsFiltered> AveregedList = new List<AwardsFiltered>();
            List<AwardsFiltered> ResultCopy = Result.ToList();

            if (AverageAwards)
            {

                foreach (AwardsFiltered item in ResultCopy)
                {
                    var exist = AveregedList.Where(m => m.CountryId == item.CountryId && m.Year == item.Year && m.Band == item.Band && m.Id != item.Id).FirstOrDefault();
                    if (exist == null)
                    {
                        if ((item.BandPaired != "" || item.BandPaired != null)
                            && (item.BandUnPaired != "" || item.BandUnPaired != null)
                            && item.BandPaired == item.BandUnPaired)
                        {
                            if (SumBand == "p")
                                item.Price /= Convert.ToDouble(item.BlockPaired);
                            else if (SumBand == "u")
                                item.Price /= Convert.ToDouble(item.BlockUnPaired);
                            else if (SumBand == "pu")
                                item.Price /= Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                        }
                        else
                        {
                            if (item.BandPaired != "" && item.BandPaired != null && (item.BandUnPaired == "" || item.BandUnPaired == null))
                            {
                                item.Price /= Convert.ToDouble(item.BlockPaired);
                            }
                            else if (item.BandUnPaired != "" && item.BandUnPaired != null && (item.BandPaired == "" || item.BandPaired == null))
                            {
                                item.Price /= Convert.ToDouble(item.BlockUnPaired);
                            }
                        }



                        List<AwardsFiltered> currentItemAwards = Result.Where(m => m.CountryId == item.CountryId && m.Year == item.Year && m.Band == item.Band && m.Id != item.Id).ToList();
                        foreach (AwardsFiltered common in currentItemAwards)
                        {
                            Result = Result.Where(m => m.Id != common.Id).ToList();
                            if ((common.BandPaired != "" || common.BandPaired != null) && (common.BandUnPaired != "" || common.BandUnPaired != null)
                                && common.BandPaired == common.BandUnPaired)
                            {
                                if (SumBand == "p")
                                    common.Price /= Convert.ToDouble(common.BlockPaired);
                                else if (SumBand == "u")
                                    common.Price /= Convert.ToDouble(common.BlockUnPaired);
                                else if (SumBand == "pu")
                                    common.Price /= Convert.ToDouble(common.BlockUnPaired) + Convert.ToDouble(common.BlockPaired);
                            }
                            else
                            {
                                if (common.BandPaired != "" && common.BandPaired != null && (common.BandUnPaired == "" || common.BandUnPaired == null))
                                {
                                    common.Price /= Convert.ToDouble(common.BlockPaired);
                                }
                                else if (common.BandUnPaired != "" && common.BandUnPaired != null && (common.BandPaired == "" || common.BandPaired == null))
                                {
                                    common.Price /= Convert.ToDouble(common.BlockUnPaired);
                                }
                            }


                            item.Price += common.Price;

                        }
                        item.Price = item.Price / (currentItemAwards.Count + 1);
                        item.NumberOfAwards = (currentItemAwards.Count + 1);
                        AveregedList.Add(item);
                    }
                }

                Result = AveregedList;
            }
            else if (AverageSumPricesAndMHZ)
            {

                foreach (AwardsFiltered item in ResultCopy)
                {
                    var x = item;
                    var exist = AveregedList.Where(m => m.CountryId == item.CountryId && m.Year == item.Year && m.Band == item.Band).FirstOrDefault();
                    if (exist == null)
                    {
                        List<AwardsFiltered> currentItemAwards = Result.Where(m => m.CountryId == item.CountryId && m.Year == item.Year && m.Band == item.Band && m.Id != item.Id).ToList();
                        double TotalMHZ = 0;
                        double? POP = 0;
                        if (item.Regionalicense == false)
                            POP = item.Pop;

                        if ((item.BandPaired != "" || item.BandPaired != null)
                        && (item.BandUnPaired != "" || item.BandUnPaired != null)
                        && item.BandPaired == item.BandUnPaired)
                        {
                            if (SumBand == "p")
                                TotalMHZ = Convert.ToDouble(item.BlockPaired);
                            else if (SumBand == "u")
                                TotalMHZ = Convert.ToDouble(item.BlockUnPaired);
                            else if (SumBand == "pu")
                                TotalMHZ = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                        }
                        else
                        {
                            if (item.BandPaired != "" && item.BandPaired != null && (item.BandUnPaired == "" || item.BandUnPaired == null))
                            {
                                TotalMHZ = Convert.ToDouble(item.BlockPaired);
                            }
                            else if (item.BandUnPaired != "" && item.BandUnPaired != null && (item.BandPaired == "" || item.BandPaired == null))
                            {
                                TotalMHZ = Convert.ToDouble(item.BlockUnPaired);
                            }
                        }


                        double TotalPrice = item.Price == null ? 0 : item.Price.Value;


                        foreach (AwardsFiltered common in currentItemAwards)
                        {
                            Result = Result.Where(m => m.Id != common.Id).ToList();
                            double value = 0;

                            if (common.Regionalicense == false)
                                POP = item.Pop;

                            if ((common.BandPaired != "" || common.BandPaired != null)
                            && (item.BandUnPaired != "" || common.BandUnPaired != null)
                            && common.BandPaired == common.BandUnPaired)
                            {
                                if (SumBand == "p")
                                    value = Convert.ToDouble(common.BlockPaired);
                                else if (SumBand == "u")
                                    value = Convert.ToDouble(common.BlockUnPaired);
                                else if (SumBand == "pu")
                                    value = Convert.ToDouble(common.BlockUnPaired) + Convert.ToDouble(common.BlockPaired);
                            }
                            else
                            {
                                if (common.BandPaired != "" && common.BandPaired != null && (common.BandUnPaired == "" || common.BandUnPaired == null))
                                {
                                    value = Convert.ToDouble(common.BlockPaired);
                                }
                                else if (common.BandUnPaired != "" && common.BandUnPaired != null && (common.BandPaired == "" || common.BandPaired == null))
                                {
                                    value = Convert.ToDouble(common.BlockUnPaired);
                                }
                            }
                            TotalMHZ += value;
                            TotalPrice += common.Price == null ? 0 : common.Price.Value;

                        }

                        if (TotalMHZ != 0)
                            item.Price = TotalPrice / TotalMHZ;
                        else
                            item.Price = 0;

                        //if (POP == 0)
                        //{
                        //    var _pop = await IScocioEconomicRepository2.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                        //     IsIMF);
                        //}
                        item.Pop = POP;
                        item.NumberOfAwards = currentItemAwards.Count + 1;
                        AveregedList.Add(item);
                    }
                }

                Result = AveregedList;
            }
                
            return Result;
        }

        [HttpGet]
        [Route("GetAllForView")]
        public async Task<IActionResult> GetAllForView()
        {

            var Result = await this.IAwardRepository.GetAllForView();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.IAwardRepository.GetById(Id);
            return Ok(Result);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            var Result = await this.IAwardRepository.GetById(Id);
            Result.IsDeleted = true;
            var Res = await IAwardRepository.Update(Result);
            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Delete),
                Page = "Award",
                PageAr = "جائزة",
                Details = ("Award  for Year(" + Result.Year + ") is Delete"),
                DetailsAr = ("تم مسح جائزة (" + Result.Year + ")"),
                Date = DateTime.Now
            });
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetFilterAwards")]
        public async Task<IActionResult> GetFilterAwards(AwardFilter_View view)
        {
            var Result = await this.IAwardRepository.GetFilteredAwards(view);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }

        [HttpPost]
        [Route("RemoveAllAwards")]
        public async Task<IActionResult> RemoveAllAwards()
        {
            var Result = await this.IAwardRepository.RemoveAllAwards();
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item1);
        }

        [HttpPost]
        [Route("BenchmarkFiltering")]
        public async Task<IActionResult> BenchmarkFiltering(ValuationBody PricingFilterBody)
        {

            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
              PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor, PricingFilterBody.AdjustByInflationFactor,
              PricingFilterBody.CountryIds, UserId);

            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
              && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
              && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();

            List<AwardsFiltered> filtered = new List<AwardsFiltered>();
            foreach (AwardsFiltered item in Result)
            {
                if (item.Pairing == "p")
                {
                    if (item.BlockPaired != "" && item.BlockPaired != null)
                        filtered.Add(item);
                }
                else if (item.Pairing == "u")
                {
                    if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                        filtered.Add(item);
                }
                else if (item.Pairing == "p+u")
                {

                    if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                        item.BlockPaired != "")
                        filtered.Add(item);
                }
            }

            Result = filtered.ToList();

            if (PricingFilterBody.RegionalLicense)
                Result = IncludeRegional(Result.ToList());

            FilterModel_View filterView = new FilterModel_View();

            filterView.RegionalLicense = PricingFilterBody.RegionalLicense;
            filterView.MinGDP = PricingFilterBody.MinGDP;
            filterView.MaxGDP = PricingFilterBody.MaxGDP;
            filterView.IsPairedAndUnPaired = PricingFilterBody.IsPairedAndUnPaired;
            filterView.IsPaired = PricingFilterBody.IsPaired;
            filterView.IsUnPaired = PricingFilterBody.IsUnPaired;
            filterView.Band = PricingFilterBody.Band;
            filterView.IsSingle = PricingFilterBody.IsSingle;

            Result = PerformFiltering(Result.ToList(), filterView);


            AdjustmentModel_View model = new AdjustmentModel_View();

            model.IsIncludeAnnual = PricingFilterBody.IsIncludeAnnual;
            model.DiscountRate = PricingFilterBody.DiscountRate;
            model.ISIMF = PricingFilterBody.ISIMF;
            model.AdjustByInflationFactor = PricingFilterBody.AdjustByInflationFactor;
            model.AdjustByPPPFactor = PricingFilterBody.AdjustByPPPFactor;
            model.AnnualizePrice = PricingFilterBody.AnnualizePrice;
            model.Term = PricingFilterBody.Term;
            model.AdjustByGDPFactor = PricingFilterBody.AdjustByGDPFactor;
            model.UniqueAwards = PricingFilterBody.UniqueAwards;
            model.SumBand = PricingFilterBody.SumBand;


            Result = PerformAdjustments(Result.ToList(), model);

            Result = Result.Where(m => m.Price != double.PositiveInfinity ).ToList();


            Result = PerformAveraging(Result.ToList(), PricingFilterBody.AverageAwards, PricingFilterBody.AverageSumPricesAndMHZ, PricingFilterBody.SumBand, PricingFilterBody.ISIMF);
            
            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
            && (m.Price != double.PositiveInfinity)).ToList();

           // Result = Result.Where(m => m.Price != null && m.Price != double.PositiveInfinity).ToList();

            if (PricingFilterBody.AverageAwards || PricingFilterBody.AverageSumPricesAndMHZ)
            {
                foreach (AwardsFiltered item in Result)
                {
                    if (item.Regionalicense == true)
                    {
                        var Pop = await IScocioEconomicRepository.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                           PricingFilterBody.ISIMF);
                        decimal? population = Pop.Item2;
                        if (PricingFilterBody.ISIMF == false)
                        {
                            population = population == null ? null : population / 1000000;
                        }
                        item.Pop = Convert.ToDouble(population);
                    }
                }
            }


            Result = Result.Where(m => m.Price != null && m.Price != double.PositiveInfinity).ToList();

            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
            && (m.Price != double.PositiveInfinity && m.Price != 0)).ToList();

            List<Benchmark_View> benchMarkResult = new List<Benchmark_View>();
            foreach (AwardsFiltered item in Result)
            {
                if (benchMarkResult.Where(m => m.Band == item.Band).ToList().Count() == 0)
                {

                    List<XYPoint2> points = new List<XYPoint2>();
                    List<AwardsFiltered> tmpList = Result.Where(m => m.Band == item.Band).ToList();


                    ExcludeOtliers_View view = new ExcludeOtliers_View();
                    view.HasPercentile = PricingFilterBody.HasPercentile;
                    view.HasQuartile = PricingFilterBody.HasQuartile;
                    view.HasRegression = PricingFilterBody.HasRegression;
                    view.HasStandardDeviation = PricingFilterBody.HasStandardDeviation;
                    view.AutoFiltering = PricingFilterBody.AutoFiltering;
                    view.KValue = PricingFilterBody.KValue;
                    view.UpperPercentile = PricingFilterBody.UpperPercentile;
                    view.LowerPercentile = PricingFilterBody.LowerPercentile;
                    view.StandardDeviationValue = PricingFilterBody.StandardDeviationValue;
                    view.Regression = PricingFilterBody.Regression;

                    tmpList = PerfomPricing(tmpList.ToList(), view, PricingFilterBody.AdjustByGDPFactor);

                    if (PricingFilterBody.UniqueAwards == true)
                    {
                        foreach (AwardsFiltered AwardsFiltered in tmpList)
                        {
                            if (AwardsFiltered.Regionalicense)
                                AwardsFiltered.Pop = AwardsFiltered.AwardPop;

                        }
                    }

                    foreach (AwardsFiltered PricingFiltered in tmpList)
                    {
                        if (PricingFiltered.GDP != null)
                            points.Add(new XYPoint2() { X = (PricingFiltered.GDP.Value), Y = (double)PricingFiltered.Price, Id = PricingFiltered.Id, Price = (double)PricingFiltered.Price });
                    }
                    double a = 0;
                    double b = 0;
                    double ssreg = 0;
                    double ssot = 0;
                    double RSQ = 0;
                    double mean = 0;
                    double median = 0;
                    string Way;
                    if (points != null && points.Count > 0)
                    {
                        List<XYPoint2> bestFit = GenerateLinearBestFit2(points, PricingFilterBody.EnforeBPositive, out a, out b, out Way);

                        mean = bestFit.Average(point => point.Price);
                        ssreg = bestFit.Sum(point => Math.Pow((double)(point.Price - point.Y), 2));
                        ssot = bestFit.Sum(point => Math.Pow((double)(point.Price - mean), 2));
                        RSQ = 1 - (ssreg / ssot);

                        int numberCount = bestFit.Count();
                        int halfIndex = bestFit.Count() / 2;
                        var sortedList = bestFit.OrderBy(n => n.Price);

                        if ((numberCount % 2) == 0)
                        {
                            double Price1 = sortedList.ElementAt(halfIndex).Price;
                            double Price2 = sortedList.ElementAt(halfIndex - 1).Price;
                            median = (Price1 + Price2) / 2;
                        }
                        else
                        {
                            median = sortedList.ElementAt(halfIndex).Price;
                        }

                    }
                    Benchmark_View _view = new Benchmark_View();
                    _view.Band = item.Band;
                    _view.Avalue = a;
                    _view.Bvalue = b;
                    _view.ssreg = ssreg;
                    _view.NumberOfAwards = points.Count();
                    _view.ssot = ssot;
                    _view.RSQ = RSQ;
                    _view.mean = mean;
                    _view.median = median;

                    benchMarkResult.Add(_view);
                }

            }
            benchMarkResult = benchMarkResult.Where(m => m.NumberOfAwards > 3).ToList();

            //  Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 || m.GDP != null)).ToList();
            return Ok(benchMarkResult);
        }


        [HttpPost]
        [Route("BenchmarkByRatio")]
        public async Task<IActionResult> BenchmarkByRatio(ValuationBody PricingFilterBody)
        {
            List<Benchmark_View> benchMarkResult = new List<Benchmark_View>();
            try { 
                var TokenClaims = HttpContext.User;
                int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);



                var Result = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
                  PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor, PricingFilterBody.AdjustByInflationFactor,
                  PricingFilterBody.CountryIds, UserId);

                Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
                && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
                && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();

                List<AwardsFiltered> filtered = new List<AwardsFiltered>();
                foreach (AwardsFiltered item in Result)
                {
                    if (item.Pairing == "p")
                    {
                        if (item.BlockPaired != "" && item.BlockPaired != null)
                            filtered.Add(item);
                    }
                    else if (item.Pairing == "u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                            filtered.Add(item);
                    }
                    else if (item.Pairing == "p+u")
                    {
                        if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                            item.BlockPaired != "")
                            filtered.Add(item);
                    }
                }

                Result = filtered.ToList();

                if (PricingFilterBody.RegionalLicense)
                    Result = IncludeRegional(Result.ToList());

                FilterModel_View filterView = new FilterModel_View();

                filterView.RegionalLicense = PricingFilterBody.RegionalLicense;
                filterView.MinGDP = PricingFilterBody.MinGDP;
                filterView.MaxGDP = PricingFilterBody.MaxGDP;
                filterView.IsPairedAndUnPaired = PricingFilterBody.IsPairedAndUnPaired;
                filterView.IsPaired = PricingFilterBody.IsPaired;
                filterView.IsUnPaired = PricingFilterBody.IsUnPaired;
                filterView.Band = PricingFilterBody.Band;
                filterView.IsSingle = PricingFilterBody.IsSingle;

                Result = PerformFiltering(Result.ToList(), filterView);


                AdjustmentModel_View model = new AdjustmentModel_View();

                model.IsIncludeAnnual = PricingFilterBody.IsIncludeAnnual;
                model.DiscountRate = PricingFilterBody.DiscountRate;
                model.ISIMF = PricingFilterBody.ISIMF;
                model.AdjustByInflationFactor = PricingFilterBody.AdjustByInflationFactor;
                model.AdjustByPPPFactor = PricingFilterBody.AdjustByPPPFactor;
                model.AnnualizePrice = PricingFilterBody.AnnualizePrice;
                model.Term = PricingFilterBody.Term;
                model.AdjustByGDPFactor = PricingFilterBody.AdjustByGDPFactor;
                model.UniqueAwards = PricingFilterBody.UniqueAwards;
                model.SumBand = PricingFilterBody.SumBand;


                Result = PerformAdjustments(Result.ToList(), model);


                Result = Result.Where(m => m.Price != double.PositiveInfinity).ToList();

                Result = PerformAveraging(Result.ToList(), PricingFilterBody.AverageAwards, PricingFilterBody.AverageSumPricesAndMHZ, PricingFilterBody.SumBand, PricingFilterBody.ISIMF);

                Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
                && (m.Price != double.PositiveInfinity)).ToList();

                if (PricingFilterBody.AverageAwards || PricingFilterBody.AverageSumPricesAndMHZ)
                {
                    foreach (AwardsFiltered item in Result)
                    {
                        if (item.Regionalicense == true)
                        {
                            var Pop = await IScocioEconomicRepository.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                               PricingFilterBody.ISIMF);
                            decimal? population = Pop.Item2;
                            if (PricingFilterBody.ISIMF == false)
                            {
                                population = population == null ? null : population / 1000000;
                            }
                            item.Pop = Convert.ToDouble(population);
                        }
                    }
                }

                Result = Result.Where(m => m.Price != null && m.Price != double.PositiveInfinity).ToList();
                Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
                    && (m.Price != double.PositiveInfinity && m.Price != 0)).ToList();

            
                foreach (AwardsFiltered item in Result)
                {
                    if (benchMarkResult.Where(m => m.Band == item.Band).ToList().Count() == 0)
                    {

                        List<AwardsFiltered> tmpList = Result.Where(m => m.Band == item.Band).ToList();


                        ExcludeOtliers_View view = new ExcludeOtliers_View();
                        view.HasPercentile = PricingFilterBody.HasPercentile;
                        view.HasQuartile = PricingFilterBody.HasQuartile;
                        view.HasRegression = PricingFilterBody.HasRegression;
                        view.HasStandardDeviation = PricingFilterBody.HasStandardDeviation;
                        view.AutoFiltering = PricingFilterBody.AutoFiltering;
                        view.KValue = PricingFilterBody.KValue;
                        view.UpperPercentile = PricingFilterBody.UpperPercentile;
                        view.LowerPercentile = PricingFilterBody.LowerPercentile;
                        view.StandardDeviationValue = PricingFilterBody.StandardDeviationValue;
                        view.Regression = PricingFilterBody.Regression;

                        tmpList = PerfomPricing(tmpList.ToList(), view, PricingFilterBody.AdjustByGDPFactor);

                        if (PricingFilterBody.UniqueAwards == true)
                        {
                            foreach (AwardsFiltered AwardsFiltered in tmpList)
                            {
                                if (AwardsFiltered.Regionalicense)
                                    AwardsFiltered.Pop = AwardsFiltered.AwardPop;

                            }
                        }


                        double? mean = tmpList.Average(point => point.Price);
                        double? median = 0;

                        int numberCount = tmpList.Count();
                        int halfIndex = tmpList.Count() / 2;
                        var sortedList = tmpList.OrderBy(n => n.Price);
                        if(sortedList.Count() > 0)
                        {
                            if ((numberCount % 2) == 0)
                            {
                                double? Price1 = sortedList.ElementAt(halfIndex) != null ? sortedList.ElementAt(halfIndex).Price : 0;
                                double? Price2 = sortedList.ElementAt(halfIndex - 1) != null ? sortedList.ElementAt(halfIndex - 1).Price : 0;
                                median = (Price1 + Price2) / 2;
                            }
                            else
                            {
                                median = sortedList.ElementAt(halfIndex).Price;
                            }
                        }
                        
                        Benchmark_View _view = new Benchmark_View();
                        _view.Band = item.Band;
                        _view.Avalue = 0;
                        _view.Bvalue = 0;
                        _view.ssreg = 0;
                        _view.NumberOfAwards = tmpList.Count();
                        _view.ssot = 0;
                        _view.RSQ = 0;
                       _view.mean = mean == null ? 0 : (double)mean;
                        _view.median = median == null ? 0 : (double)median;

                        benchMarkResult.Add(_view);
                    }

                }

                    ///  benchMarkResult = benchMarkResult.Where(m => m.NumberOfAwards > 3).ToList();
            }
            catch(Exception e)
            {

            }

            return Ok(benchMarkResult);
        }


        public static List<AwardsFiltered> IncludeRegional(List<AwardsFiltered> AwardsFiltered)
        {
            List<AwardsFiltered> FR = new List<AwardsFiltered>();

            foreach (AwardsFiltered item in AwardsFiltered)
            {
                if (item.Regionalicense == false)
                    FR.Add(item);
                else
                {
                    if (item.AwardPop > 0)
                        FR.Add(item);
                }
            }
            return FR;
        }

        [HttpPost]
        [Route("RecalculateValuation")]
        public async Task<IActionResult> RecalculateValuation(Regression_Recalculate PricingFilterBody)
        {
            
            List <AwardsFiltered> Result = PricingFilterBody.AwardsFiltered;

            List<XYPoint2> points = new List<XYPoint2>();
            foreach (AwardsFiltered PricingFiltered in Result)
            {
                if (PricingFiltered.GDP != null)
                    points.Add(new XYPoint2() { X = (PricingFiltered.GDP.Value), Y = (double)PricingFiltered.Price, Id = PricingFiltered.Id, Price = (double)PricingFiltered.Price });
            }
            double a, b;
            string Way;
            if (points != null && points.Count > 0)
            {
                List<XYPoint2> bestFit = GenerateLinearBestFit2(points, PricingFilterBody.EnforeBPositive, out a, out b, out Way);

                var GDP = await IScocioEconomicRepository.GetGDPForValuations(PricingFilterBody.IssueDate, Convert.ToInt32(PricingFilterBody.CountryId),
                    PricingFilterBody.IsPPP, PricingFilterBody.ISIMF);

                var POP = await IScocioEconomicRepository.GetPopForValuations(PricingFilterBody.IssueDate, Convert.ToInt32(PricingFilterBody.CountryId),
                   PricingFilterBody.ISIMF);

                double _FianlGDP = PricingFilterBody.ISIMF == true ? (double)GDP.Item2 * 1000 : (double)GDP.Item2;
                double? _Pop = POP.Item2 == null ? null : (double)POP.Item2;
                if (PricingFilterBody.ISIMF == false && _Pop != null)
                {
                    _Pop = _Pop / 1000000;
                }
                Country country = await ICountryRepository.GetById(Convert.ToInt32(PricingFilterBody.CountryId));
                AwardsFiltered predicted = new AwardsFiltered();
                predicted.Id = 0;
                predicted.CountryId = Convert.ToInt32(PricingFilterBody.CountryId);
                predicted.Pop = PricingFilterBody.PopCovered == null ? _Pop : PricingFilterBody.PopCovered;
                predicted.Year = PricingFilterBody.IssueDate;
                predicted.Price = (double)(((double)_FianlGDP) * (double)a) + (double)b;
                predicted.GDP = (decimal)_FianlGDP;
                predicted.CountryName = PricingFilterBody.Lang == "ar" ? country.NameAr : country.NameEn;
                predicted.CountryId = Convert.ToInt32(PricingFilterBody.CountryId);
                predicted.OperatorName = "";
                predicted.Month = 0;
                predicted.AuctionDateMonth = 0;
                predicted.AuctionDateYear = 0;
                predicted.AnnualFees = 0;
                predicted.UpFrontFees = 0;
                predicted.ReservePrice = 0;
                predicted.Terms = PricingFilterBody.Terms.Replace(",", "/");
                predicted.Band = PricingFilterBody.Band.Replace(",", "/");
                predicted.Pairing = "";
                predicted.MHZ = "";
                predicted.Regionalicense = false;
                predicted.Paid = 0;
                predicted.coverage = "";
                predicted.SingleOrMultiBand = 'S';
                predicted.BandPaired = "";
                predicted.BandUnPaired = "";
                predicted.Id = 0;
                predicted.InflationFactor = 0;
                predicted.PPPFactor = 0;
                predicted.GDPValue = 0;
                predicted.BlockPaired = "";
                predicted.BlockUnPaired = "";

                predicted.CalculatedMHZ = 0;
                predicted.bandCountry = "";

                List<AwardsFiltered> lst = new List<AwardsFiltered>();
                lst = Result.ToList();

                lst.Insert(0, predicted);

                foreach (AwardsFiltered item in lst)
                {
                    item.aValue = (double)a;
                    item.bValue = (double)b;
                }

                return Ok(lst);

            }
        
            else
            {
                List<AwardsFiltered> Result2 = new List<AwardsFiltered>();
                return Ok(Result2);
            }
                
        }

        [HttpPost]
        [Route("FilterTrends")]
        public async Task<IActionResult> FilterTrends(PricingFilterBody PricingFilterBody)
        {

            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IAwardRepository.FilterAwards2(PricingFilterBody.Lang, PricingFilterBody.IsPPP, PricingFilterBody.ISIMF, PricingFilterBody.FromYear, PricingFilterBody.ToYear,
                PricingFilterBody.IssueDate, PricingFilterBody.AdjustByPPPFactor, PricingFilterBody.AdjustByInflationFactor,
                PricingFilterBody.CountryIds, UserId);


            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
            && (m.Terms != null && m.Terms != "") && m.UpFrontFees != null && m.UpFrontFees != 0
            && (m.BandPaired != "" || m.BandUnPaired != "")).ToList();



            List<AwardsFiltered> filtered = new List<AwardsFiltered>();
            foreach (AwardsFiltered item in Result)
            {
                if (item.Pairing == "p")
                {
                    if (item.BlockPaired != "" && item.BlockPaired != null)
                        filtered.Add(item);
                }
                else if (item.Pairing == "u")
                {
                    if (item.BlockUnPaired != "" && item.BlockUnPaired != null)
                        filtered.Add(item);
                }
                else if (item.Pairing == "p+u")
                {
                    if (item.BlockUnPaired != "" && item.BlockUnPaired != null && item.BlockPaired != "" &&
                        item.BlockPaired != "")
                        filtered.Add(item);
                }
            }

            Result = filtered.ToList();

            if (PricingFilterBody.RegionalLicense)
                Result = IncludeRegional(Result.ToList());

            FilterModel_View filterView = new FilterModel_View();

            filterView.RegionalLicense = PricingFilterBody.RegionalLicense;
            filterView.MinGDP = PricingFilterBody.MinGDP;
            filterView.MaxGDP = PricingFilterBody.MaxGDP;
            filterView.IsPairedAndUnPaired = PricingFilterBody.IsPairedAndUnPaired;
            filterView.IsPaired = PricingFilterBody.IsPaired;
            filterView.IsUnPaired = PricingFilterBody.IsUnPaired;
            filterView.Band = PricingFilterBody.Band;
            filterView.IsSingle = PricingFilterBody.IsSingle;

            Result = PerformFiltering(Result.ToList(), filterView);



            AdjustmentModel_View model = new AdjustmentModel_View();
            model.IsIncludeAnnual = PricingFilterBody.IsIncludeAnnual;
            model.DiscountRate = PricingFilterBody.DiscountRate;
            model.ISIMF = PricingFilterBody.ISIMF;
            model.AdjustByInflationFactor = PricingFilterBody.AdjustByInflationFactor;
            model.AdjustByPPPFactor = PricingFilterBody.AdjustByPPPFactor;
            model.AnnualizePrice = PricingFilterBody.AnnualizePrice;
            model.Term = PricingFilterBody.Term;
            model.AdjustByGDPFactor = PricingFilterBody.AdjustByGDPFactor;
            model.UniqueAwards = PricingFilterBody.UniqueAwards;
            model.SumBand = PricingFilterBody.SumBand;
            Result = PerformAdjustments(Result.ToList(), model);


            Result = Result.Where(m => m.Price != double.PositiveInfinity).ToList();

            Result = PerformAveraging(Result.ToList(), PricingFilterBody.AverageAwards, PricingFilterBody.AverageSumPricesAndMHZ, PricingFilterBody.SumBand, PricingFilterBody.ISIMF);


            if (PricingFilterBody.AverageAwards || PricingFilterBody.AverageSumPricesAndMHZ)
            {
                foreach (AwardsFiltered item in Result)
                {
                    if (item.Regionalicense == true)
                    {
                        var Pop = await IScocioEconomicRepository.GetPopForValuations(Convert.ToInt32(item.Year), Convert.ToInt32(item.CountryId),
                           PricingFilterBody.ISIMF);
                        decimal? population = Pop.Item2;
                        if (PricingFilterBody.ISIMF == false)
                        {
                            population = population == null ? null : population / 1000000;
                        }
                        item.Pop = Convert.ToDouble(population);
                    }
                }
            }

          
            Result = Result.Where(m => m.Price != double.PositiveInfinity && m.Price != 0).ToList();

            Result = Result.Where(m => (m.Pop > 0 || m.AwardPop > 0) && (m.GDP > 0 && m.GDP != null)
            && (m.Price != double.PositiveInfinity)).ToList();

            List<AwardsFiltered> lstAwards = new List<AwardsFiltered>();
            foreach (AwardsFiltered item in Result)
            {
                if (lstAwards.Where(m => m.Band == item.Band).ToList().Count() == 0)
                {

                    List<AwardsFiltered> tmpList = Result.Where(m => m.Band == item.Band).ToList();


                    ExcludeOtliers_View view = new ExcludeOtliers_View();
                    view.HasPercentile = PricingFilterBody.HasPercentile;
                    view.HasQuartile = PricingFilterBody.HasQuartile;
                    view.HasRegression = PricingFilterBody.HasRegression;
                    view.HasStandardDeviation = PricingFilterBody.HasStandardDeviation;
                    view.AutoFiltering = PricingFilterBody.AutoFiltering;
                    view.KValue = PricingFilterBody.KValue;
                    view.UpperPercentile = PricingFilterBody.UpperPercentile;
                    view.LowerPercentile = PricingFilterBody.LowerPercentile;
                    view.StandardDeviationValue = PricingFilterBody.StandardDeviationValue;
                    view.Regression = PricingFilterBody.Regression;

                    tmpList = PerfomPricing(tmpList.ToList(), view, PricingFilterBody.AdjustByGDPFactor);

                    lstAwards.AddRange(tmpList);
                }

            }

            Result = lstAwards;


            if (PricingFilterBody.UniqueAwards == true)
            {
                foreach (AwardsFiltered AwardsFiltered in Result)
                {
                    if (AwardsFiltered.Regionalicense)
                        AwardsFiltered.Pop = AwardsFiltered.AwardPop;

                }
            }


            return Ok(Result);
        }
    }
}