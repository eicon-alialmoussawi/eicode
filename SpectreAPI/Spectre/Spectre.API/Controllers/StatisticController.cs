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
    [Route("api/Statistics")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StatisticController : ControllerBase
    {

        private readonly IStatisticRepository IStatisticRepository;
        private readonly ILogger ILogger;

        public StatisticController(ILogger ILogger, IStatisticRepository IStatisticRepository)
        {
            this.IStatisticRepository = IStatisticRepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetStatistics")]
        public async Task<IActionResult> GetStatistics()
        {

            var Result = await IStatisticRepository.GetStatistics();
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }

        [HttpGet]
        [Route("GetForBarChart")]
        public async Task<IActionResult> GetTotalSales()
        {

            var sales = await IStatisticRepository.GetTotalSales();
            var customers = await IStatisticRepository.GetTotalCustomersPerPackage();
            var active = await IStatisticRepository.ActivePackagesStatistics();
            if (!sales.Item1 || !customers.Item1 || !active.Item1)
                return BadRequest();

            Statistics_View2 view = new Statistics_View2();
            view.sales = sales.Item2;
            view.customers = customers.Item2;
            view.active = active.Item2;

            return Ok(view);
        }

    }
}
