using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Spectre.API.Controllers
{
    [Route("api/Region")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class RegionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IRegionRepository IRegionRepository;
        private readonly ILogger ILogger;
        public RegionController(IMapper mapper, ILogger ILogger, IRegionRepository IRegionRepository)
        {
            this.IRegionRepository = IRegionRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.IRegionRepository.GetById(Id);
            return Ok(Result);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(RegionDetails regionDetails)
        {
          
          var res = await IRegionRepository.Create(regionDetails);


            return Ok("Success");
        }

        

        [HttpGet]
        [Route("GetRegions")]
        public async Task<IActionResult> GetRegions()
        {

            var Result = await this.IRegionRepository.GetRegions();
            return Ok(Result);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {

            var res = await IRegionRepository.Delete(Id);


            return Ok("Success");
        }

    }
}
