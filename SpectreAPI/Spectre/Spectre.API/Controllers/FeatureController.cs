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
using Spectre.Core.Repositories;

namespace Spectre.API.Controllers
{
    [Route("api/Features")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureRepository IFeatureRepository;
        private readonly ILogger ILogger;
        public FeatureController(ILogger ILogger, IFeatureRepository IFeatureRepository)
        {
            this.IFeatureRepository = IFeatureRepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await IFeatureRepository.GetAll();
            if (!Result.Item1)
                return BadRequest();

            return Ok(Result.Item2);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {

            var Result = await IFeatureRepository.Delete(Id);
            if (!Result.Item1)
                return BadRequest();

            return Ok(Result.Item1);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save(Feature Feature)
        {

            var Result = await IFeatureRepository.Save(Feature);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item1);
        }

    }
}
