using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Net;

using System.Threading.Tasks;

using Spectre.API.Utilities;
using Spectre.Core.Models.Extenders;
using System.Globalization;


namespace Spectre.API.Controllers
{
    [Route("api/Bnechmark")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class BenchmarkController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAwardRepository IAwardRepository;
        private readonly ILogger ILogger;
        public readonly ICountryRepository ICountryRepository;
        public readonly ILookupRepository ILookupRepository;
        public BenchmarkController(IMapper mapper, ILogger ILogger, IAwardRepository IAwardRepository, ICountryRepository ICountryRepository, ILookupRepository ILookupRepository)
        {
            this.IAwardRepository = IAwardRepository;
            this.ILookupRepository = ILookupRepository;
            this.ICountryRepository = ICountryRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }
        public class XYPoint
        {
            public decimal X;
            public decimal Y;
            public int Id;
            public decimal Price;
        }


        [HttpPost]
        [Route("Filter")]
        public async Task<IActionResult> Filter(PricingFilterBody PricingFilterBody)
        {

            var Result = await this.IAwardRepository.FilterAwards(PricingFilterBody.IsPPP, PricingFilterBody.ISIMF,
                 PricingFilterBody.IsSingle, PricingFilterBody.IsMultiple, PricingFilterBody.IsPaired, PricingFilterBody.IsPairedAndUnPaired, PricingFilterBody.RegionalLicense,
                 PricingFilterBody.FromYear, PricingFilterBody.ToYear, PricingFilterBody.MaxGDP, PricingFilterBody.MinGDP, PricingFilterBody.CountryIds,
                 PricingFilterBody.Band, PricingFilterBody.SourceId, PricingFilterBody.IsUnPaired, PricingFilterBody.IssueDate);
            if (PricingFilterBody.RegionalLicense == false)
                Result = Result.Where(m => m.Regionalicense == false).ToList();
            if (PricingFilterBody.MinGDP >= 0 && PricingFilterBody.MaxGDP >= 0)
                Result = Result.Where(m => (m.GDP == null || m.GDP >= PricingFilterBody.MinGDP) && (m.GDP == null || m.GDP <= PricingFilterBody.MaxGDP)).ToList();


            if (!PricingFilterBody.IsPairedAndUnPaired)
                Result = Result.Where(m => m.Pairing != "b").ToList();
            if (!PricingFilterBody.IsPaired)
                Result = Result.Where(m => m.Pairing != "p").ToList();
            if (!PricingFilterBody.IsUnPaired)
                Result = Result.Where(m => m.Pairing != "u").ToList();
            string[] SelectedBands = PricingFilterBody.Band.Split(',');
            var FilteredResults = new List<AwardsFiltered>();
            if (PricingFilterBody.IsSingle)
            {
                if (PricingFilterBody.IsPaired)
                {
                    foreach (AwardsFiltered Award in Result)
                    {
                        if (SelectedBands.Contains(Award.BandPaired))
                            FilteredResults.Add(Award);
                    }
                }
                if (PricingFilterBody.IsUnPaired)
                {
                    foreach (AwardsFiltered Award in Result)
                    {
                        if (SelectedBands.Contains(Award.BandUnPaired) && !FilteredResults.Contains(Award))
                            FilteredResults.Add(Award);
                    }
                }
                if (PricingFilterBody.IsPairedAndUnPaired)
                {
                    foreach (AwardsFiltered Award in Result)
                    {
                        if (SelectedBands.Contains(Award.BandUnPaired) && SelectedBands.Contains(Award.BandPaired))
                            if (!FilteredResults.Contains(Award))
                                FilteredResults.Add(Award);
                    }
                    //    FilteredResults.AddRange(Result.Where(m => SelectedBands.Contains(m.BandUnPaired) && SelectedBands.Contains(m.BandPaired)).ToList());

                }
                Result = FilteredResults;
            }



            if (PricingFilterBody.IsSingle == true)
            {
                Result = Result.Where(m => m.SingleOrMultiBand == 's' || m.SingleOrMultiBand == 'S' || m.SingleOrMultiBand == 'I' || m.SingleOrMultiBand == 'i').ToList();
            }
            if (PricingFilterBody.IsIncludeAnnual)
            {
                foreach (AwardsFiltered item in Result)
                {
                    double TotalPriceP = 0;
                    if (item.AnnualFees != null)
                    {

                        int Term = int.Parse(item.Terms, 0);
                        double Summation = 0;
                        if (Term > 0)
                        {
                            for (int i = 1; i < Term; i++)
                            {
                                Summation += 1 / Math.Pow((1.0 + PricingFilterBody.DiscountRate), i);

                            }
                        }
                        double NetPresent = (double)(item.AnnualFees * Summation);

                        DateTime Fromdt = new DateTime(item.Year.Value, item.Month.Value, 1);
                        DateTime Auctiondt = new DateTime(item.Year.Value, item.Month.Value, 1);

                        TimeSpan DelaySpan = Fromdt - Auctiondt;
                        double YearsOfDelay = DelaySpan.TotalDays;
                        if (YearsOfDelay > 365)
                        {
                            DateTime zeroTime = new DateTime(1, 1, 1);
                            TimeSpan span = Fromdt - Auctiondt;
                            double YearsOfDelayFractioned = (zeroTime + span).Year - 1;
                            int Months = ((Fromdt.Year - Auctiondt.Year) * 12) + Fromdt.Month - Auctiondt.Month;
                            int DigitCount = Months.ToString().Length;
                            YearsOfDelayFractioned += Months / Math.Pow(10, (DigitCount));
                            double TotalAnnualPayment = NetPresent * Math.Pow(1 + PricingFilterBody.DiscountRate, -(YearsOfDelayFractioned));
                            double TotalPriceAtAuctionDate = (double)(item.UpFrontFees + TotalAnnualPayment);
                            TotalPriceP = TotalPriceAtAuctionDate * Math.Pow(1 + PricingFilterBody.DiscountRate, YearsOfDelayFractioned);
                        }
                        else
                        {
                            TotalPriceP = (double)(item.UpFrontFees + NetPresent);

                        }
                    }
                    else
                    {
                        TotalPriceP = (double)(item.UpFrontFees);

                    }
                    item.Price = TotalPriceP;

                }
            }
            else
            {
                foreach (AwardsFiltered item in Result)
                {
                    DateTime Fromdt = new DateTime(item.Year.Value, item.Month.Value, 1); double TotalPriceP = 0;
                    if (item.AuctionDateYear.HasValue && item.AuctionDateMonth.HasValue)
                    {
                        DateTime Auctiondt = new DateTime(item.AuctionDateYear.Value, item.AuctionDateMonth.Value, 1);

                        TimeSpan DelaySpan = Fromdt - Auctiondt;
                        double YearsOfDelay = DelaySpan.TotalDays;
                        if (YearsOfDelay > 365)
                        {
                            DateTime zeroTime = new DateTime(1, 1, 1);
                            TimeSpan span = Fromdt - Auctiondt;
                            double YearsOfDelayFractioned = (zeroTime + span).Year - 1;
                            TotalPriceP = (double)(item.UpFrontFees * Math.Pow(1 + PricingFilterBody.DiscountRate, YearsOfDelayFractioned));
                        }
                    }
                    else
                    {
                        TotalPriceP = (double)item.UpFrontFees;
                    }
                    item.Price = TotalPriceP;
                }

            }


            foreach (AwardsFiltered item in Result)
            {
                item.bandCountry = item.CountryName + "-" + item.Band + "(" + item.Year + ")";
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

                double LicenseFactor = 0;
                double Numerator = 0;
                double Denominator = 0;
                for (double t = 0.0; t < (int.Parse(item.Terms) * 12) - 1; t++)
                {
                    Numerator += Math.Pow((1.0 / (1.0 + PricingFilterBody.DiscountRate)), t / 12.0);//(1 / (Math.Pow(1 + int.Parse(item.Terms), t / 12)));
                }
                for (double t = 0.0; t < (PricingFilterBody.Term * 12) - 1; t++)
                {
                    Denominator += Math.Pow((1.0 / (1.0 + PricingFilterBody.DiscountRate)), t / 12.0);
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
                    item.Price = item.Price * PricingFilterBody.DiscountRate * (1 / (1 - (1 / (Math.Pow((1 + PricingFilterBody.DiscountRate), PricingFilterBody.Term)))));
                }
                if (PricingFilterBody.AdjustByGDPFactor)
                {
                    item.Price = item.Price / (item.GDPValue / item.Pop);
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
                        item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired + item.BlockPaired);
                    }
                }
                else
                {
                    item.CalculatedMHZ = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);
                }



            }


            List<AwardsFiltered> AveregedList = new List<AwardsFiltered>();
            List<AwardsFiltered> ResultCopy = Result.ToList();

            if (PricingFilterBody.AverageAwards)
            {

                foreach (AwardsFiltered item in ResultCopy)
                {
                    var exist = AveregedList.Where(m => m.Year == item.Year && m.BandPaired == item.BandPaired && m.BandUnPaired == item.BandUnPaired && m.Id != item.Id).FirstOrDefault();
                    if (exist == null)
                    {
                        List<AwardsFiltered> currentItemAwards = Result.Where(m => m.CountryId == item.CountryId && m.Year == item.Year && m.BandPaired == item.BandPaired && m.BandUnPaired == item.BandUnPaired && m.Id != item.Id).ToList();
                        foreach (AwardsFiltered common in currentItemAwards)
                        {
                            Result = Result.Where(m => m.Id != common.Id).ToList();
                            item.Price += common.Price;

                        }
                        if (currentItemAwards.Count > 0)
                        {
                            item.Price = item.Price / (currentItemAwards.Count + 1);
                        }
                        AveregedList.Add(item);
                    }
                }

                Result = AveregedList;
            }




            List<AwardsFiltered> AveregedList2 = new List<AwardsFiltered>();
            List<AwardsFiltered> ResultCopy2 = Result.ToList();

            if (PricingFilterBody.AverageSumPricesAndMHZ)
            {

                foreach (AwardsFiltered item in ResultCopy2)
                {
                    var exist = AveregedList2.Where(m => m.Year == item.Year && m.BandPaired == item.BandPaired && m.BandUnPaired == item.BandUnPaired && m.Id != item.Id).FirstOrDefault();
                    if (exist == null)
                    {
                        List<AwardsFiltered> currentItemAwards = Result.Where(m => m.CountryId == item.CountryId && m.Year == item.Year && m.BandPaired == item.BandPaired && m.BandUnPaired == item.BandUnPaired && m.Id != item.Id).ToList();
                        double TotalMHZ = 0;

                        if (PricingFilterBody.SumBand == "p")
                            TotalMHZ = Convert.ToDouble(item.BlockPaired);
                        else if (PricingFilterBody.SumBand == "u")
                            TotalMHZ = Convert.ToDouble(item.BlockUnPaired);
                        else if (PricingFilterBody.SumBand == "pu")
                            TotalMHZ = Convert.ToDouble(item.BlockUnPaired) + Convert.ToDouble(item.BlockPaired);

                        double TotalPrice = item.Price == null ? 0 : item.Price.Value;
                        foreach (AwardsFiltered common in currentItemAwards)
                        {
                            Result = Result.Where(m => m.Id != common.Id).ToList();
                            double value = 0;

                            if (PricingFilterBody.SumBand == "p")
                                value = Convert.ToDouble(common.BlockPaired);
                            else if (PricingFilterBody.SumBand == "u")
                                value = Convert.ToDouble(common.BlockUnPaired);
                            else if (PricingFilterBody.SumBand == "pu")
                                value = Convert.ToDouble(common.BlockUnPaired) + Convert.ToDouble(common.BlockPaired);


                            TotalMHZ += value;
                            TotalPrice += common.Price == null ? 0 : common.Price.Value;

                        }
                        if (currentItemAwards.Count > 0)
                        {
                            item.Price = TotalPrice / TotalMHZ;
                        }
                        AveregedList2.Add(item);
                    }
                }

                Result = AveregedList2;
            }




            if (PricingFilterBody.HasPercentile)
            {

                List<AwardsFiltered> SortedList = Result.OrderByDescending(o => o.Price).ToList();
                double MaxPosition = (double)((PricingFilterBody.UpperPercentile / 100) * (double)Result.Count());
                double MinPosition = (int)(PricingFilterBody.LowerPercentile / 100) * (double)Result.Count();

                int MinInd = Convert.ToInt32(MinPosition);
                int MaxInd = Convert.ToInt32(MaxPosition);
                var ValueAtMax = Result.ElementAt(MaxInd);
                var ValueAtMin = Result.ElementAt(MinInd);
                if (ValueAtMax.Price != null && ValueAtMin.Price != null)
                    Result = Result.Where(m => m.Price > ValueAtMin.Price && m.Price < ValueAtMax.Price).ToList();
            }
            if (PricingFilterBody.HasQuartile)
            {
                List<AwardsFiltered> SortedList = Result.OrderByDescending(o => o.Price).ToList();
                int MaxPosition = (int)PricingFilterBody.UpperPercentile * Result.Count();
                int MinPosition = (int)PricingFilterBody.LowerPercentile * Result.Count();
                double ValueAtMax = (double)Result.ElementAt(MaxPosition).Price;
                double ValueAtMin = (double)Result.ElementAt(MinPosition).Price;
                double IQR = ValueAtMax - ValueAtMax - ValueAtMin;
                double MinValue = ValueAtMin - (PricingFilterBody.KValue * IQR);
                double MaxValue = ValueAtMax + (PricingFilterBody.KValue * IQR);

                Result = Result.Where(m => m.Price > MinValue && m.Price < MaxValue).ToList();

            }
            if (PricingFilterBody.HasStandardDeviation)
            {
                double Mean = 0;
                double STD = 0;
                double Sum = 0;
                double Variance = 0;
                double SumSQR = 0;
                foreach (AwardsFiltered Award in Result)
                {
                    Sum += (double)Award.Price;

                }
                Mean = Sum / Result.Count();
                foreach (AwardsFiltered Award in Result)
                {

                    double deviation = (double)(Award.Price - Mean);
                    SumSQR += deviation * deviation;
                }

                Variance = SumSQR / Result.Count();

                STD = (float)Math.Sqrt(Variance);
                foreach (AwardsFiltered Award in Result)
                {

                    Result = Result.Where(m => m.Price - Mean < (PricingFilterBody.StandardDeviationValue * STD)).ToList();
                }
            }
            if (PricingFilterBody.HasRegression && PricingFilterBody.HasRegression)
            {
                List<XYPoint> points = new List<XYPoint>();
                foreach (AwardsFiltered PricingFiltered in Result)
                {
                    if (PricingFiltered.GDP != null)
                        points.Add(new XYPoint() { X = (PricingFiltered.GDP.Value), Y = (decimal)PricingFiltered.Price, Id = PricingFiltered.Id, Price = (decimal)PricingFiltered.Price });
                }
                decimal a, b;
                if (points != null && points.Count > 0)
                {
                    List<XYPoint> bestFit = Regression(points, out a, out b,true);

                    int NumberOfPoints = PricingFilterBody.Regression * Result.Count();
                    while (NumberOfPoints-- > 0)
                    {
                        decimal Max = Decimal.MinValue;
                        int PointToRemoveId = 0;
                        for (int index = 0; index < points.Count; index++)
                        {
                            decimal MSE = (decimal)Math.Pow((double)points[index].Price, 2);
                            if (Max < bestFit[index].Y)
                            {
                                Max = bestFit[index].Y;
                                PointToRemoveId = points[index].Id;
                            }
                        }
                        if (PointToRemoveId > 0)
                        {
                            foreach (XYPoint Point in points)
                            {
                                if (Point.Id == PointToRemoveId)
                                {
                                    points.Remove(Point);
                                    Result = Result.Where(m => m.Id != Point.Id).ToList();
                                }
                            }
                            bestFit = Regression(points, out a, out b,true);
                        }
                    }
                }
                // Console.WriteLine("y = {0:#.####}x {1:+#.####;-#.####}", a, -b);

            }
            foreach (AwardsFiltered AwardsFiltered in Result)
            {
                if (AwardsFiltered.Regionalicense)
                    AwardsFiltered.Pop = AwardsFiltered.AwardPop;

            }
            Result = Result.Where(m => m.Pop > 0).ToList();
            return Ok(Result);
        }


        public static List<XYPoint> Regression(List<XYPoint> points, out decimal a, out decimal b, bool EnforceBPos)
        {
            try
            {
                int numPoints = points.Count;
                decimal meanX = points.Average(point => point.X);
                decimal meanY = points.Average(point => point.Y);


                decimal Numerator = points.Sum(point => (point.X - meanX) * (point.Y - meanY));
                double Denumenator = points.Sum(point => Math.Pow(decimal.ToDouble(point.X) - decimal.ToDouble(meanX), 2));
                decimal sumXSquared = points.Sum(point => point.X * point.X);
                decimal sumXY = points.Sum(point => point.X * point.Y);

                a = Numerator / (decimal)Denumenator;
                b = meanY - a * meanX;
                if (a < 0 || (b < 0 && EnforceBPos))
                {
                    a = sumXY / sumXSquared;
                    b = 0;
                }

                decimal a1 = a;
                decimal b1 = b;

                return points.Select(point => new XYPoint() { X = point.X, Y = a1 * point.X - b1, Id = point.Id, Price = point.Price }).ToList();
            }
            catch (Exception ex)
            {
                decimal a1 = 0;
                decimal b1 = 0;
                a = 0;
                b = 0;
                return points.Select(point => new XYPoint() { X = point.X, Y = point.Y, Id = point.Id, Price = point.Price }).ToList();

            }
        }

    }
}
