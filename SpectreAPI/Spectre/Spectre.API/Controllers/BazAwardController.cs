using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/BazAwards")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BazAwardController : Controller
    {
        private readonly IBazAwardRepository IBazAwardRepository;
        public BazAwardController(IBazAwardRepository bazAwardRepository)
        {
            IBazAwardRepository = bazAwardRepository;

        }
        [HttpPost]
        [Route("GetFilteredBazAwards")]
        public async Task<IActionResult> GetFilteredBazAwards(BazAwardFilter_View view)
        {
            var Result = await this.IBazAwardRepository.GetFilteredBazAwards(view);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }

        [HttpPost]
        [Route("GetFilteredBazAwardsForPricing")]
        public async Task<IActionResult> GetFilteredBazAwardsForPricing(BazAwardFilter_ViewForPricing view)
        {
            try
            {
                // Validate input
                if (view == null)
                {

                    return BadRequest("Filter criteria cannot be null");
                }

                // Validate essential parameters
                if (view.Term <= 0)
                {

                    return BadRequest("Term must be a positive value");
                }


                // Get data from repository
                var (success, results) = await IBazAwardRepository.GetFilteredBazAwardsForPricing(view);

                if (!success)
                {

                    return BadRequest("Failed to retrieve BazAwards data");
                }

                if (results == null || !results.Any())
                {
                    return Ok(new List<BazAwardViewForPricing>());
                }

                // Configure adjustment model
                var adjustmentModel = new AdjustmentModel_View
                {
                    IsIncludeAnnual = false,
                    DiscountRate = view.DiscountRate,
                    ISIMF = true,
                    AdjustByInflationFactor = view.AdjustForInflation ?? false,
                    AdjustByPPPFactor = false,
                    AnnualizePrice = false,
                    Term = view.Term,
                    AdjustByGDPFactor = false,
                    UniqueAwards = false
                };

                // Apply adjustments
                var adjustedResults = PerformAdjustments(results, adjustmentModel);

                // Log success

                // Return results
                return Ok(adjustedResults);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        public static List<BazAwardViewForPricing> PerformAdjustments(
       List<BazAwardViewForPricing> awards,
       AdjustmentModel_View model)
        {
            // Early return for empty collections
            if (awards == null || !awards.Any())
                return new List<BazAwardViewForPricing>();

            // Fixed discount rate factor calculation - convert from percentage to decimal once
            double discountRateFactor = 1 + (model.DiscountRate / 100);

            foreach (var award in awards)
            {
                // Initialize with original values
                award.Adjusted_Price_mUSD = award.Price_mUSD;
                award.Adjust_Dollar_per_MHz_pop = award.Dollar_per_MHz_pop;

                // Calculate license term adjustment factor
                double numerator = 0;
                double denominator = 0;

                // Calculate numerator based on model term
                for (int t = 0; t < model.Term * 12; t++)
                {
                    double pow = t / 12.0;
                    numerator += Math.Pow(1 / discountRateFactor, pow);
                }

                // Fixed variable name - was _LicenseTerm which is undefined
                double licenseTerm = Convert.ToDouble(award.Term_Y) * 12;

                // Calculate denominator based on license term
                for (int t = 0; t < Convert.ToInt32(licenseTerm); t++)
                {
                    double pow = t / 12.0;
                    denominator += Math.Pow(1 / discountRateFactor, pow);
                }

                // Guard against division by zero
                double licenseFactor = denominator != 0 ? numerator / denominator : 1.0;

                // Apply license factor
                award.Adjusted_Price_mUSD *= licenseFactor;

                // Apply inflation adjustment if enabled
                if (model.AdjustByInflationFactor)
                {
                    award.Adjusted_Price_mUSD *= award.InflationFactor;
                }

                // Apply purchasing power parity adjustment if enabled
                if (model.AdjustByPPPFactor)
                {
                    award.Adjusted_Price_mUSD *= award.PPP_Factor;
                }

                // Annualize price if enabled
                if (model.AnnualizePrice)
                {
                    if (model.DiscountRate == 0)
                    {
                        // Simple division by term when discount rate is zero
                        award.Adjusted_Price_mUSD /= Convert.ToDouble(award.Term_Y);
                    }
                    else
                    {
                        // Apply annualization formula with discount rate
                        double discountRateDecimal = model.DiscountRate / 100;
                        double powerTerm = Math.Pow(1 + discountRateDecimal, model.Term);
                        award.Adjusted_Price_mUSD *= discountRateDecimal * (1 / (1 - (1 / powerTerm)));
                    }
                }

                // Apply GDP adjustment if enabled
                if (model.AdjustByGDPFactor)
                {
                    if (award.GDPc != 0)
                    {
                        award.Adjusted_Price_mUSD /= award.GDPc;
                    }
                }

                // Calculate Adjust_Dollar_per_MHz_pop based on the adjusted price
                // If the original and MHz*pop values are available, scale proportionally
                if (award.Price_mUSD != 0 && award.Dollar_per_MHz_pop != 0)
                {
                    award.Adjust_Dollar_per_MHz_pop = award.Dollar_per_MHz_pop * (award.Adjusted_Price_mUSD / award.Price_mUSD);
                }
                else if (award.Block_MHZ > 0)
                {
                    double popnumeric = ParsePopulation(award.Population) / 1000000;
                    // Direct calculation if MHz and population data are available
                    award.Adjust_Dollar_per_MHz_pop = award.Adjusted_Price_mUSD / (award.Block_MHZ * popnumeric);
                }
            }

            return awards;
        }
        private static double ParsePopulation(string populationString)
        {
            if (string.IsNullOrWhiteSpace(populationString))
                return 0;

            // Remove commas and any other non-numeric characters except decimal point
            string cleanedString = new string(populationString.Where(c => char.IsDigit(c) || c == '.').ToArray());

            // Try to parse the cleaned string
            if (double.TryParse(cleanedString, out double result))
                return result;

            return 0;
        }

        [Route("GetCountyAwardsCount")]
        public async Task<IActionResult> GetCountyAwardsCount(BazAwardFilter_View view)
        {
            var Result = await this.IBazAwardRepository.GetCountyAwardsCountAsync(view);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IBazAwardRepository.GetAll();
            return Ok(Result);
        }
    }
}
