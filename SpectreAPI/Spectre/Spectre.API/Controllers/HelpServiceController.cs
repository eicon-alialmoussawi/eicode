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

namespace Spectre.API.Controllers
{
    [Route("api/HelpService")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HelpServiceController : ControllerBase
    {
        private readonly IHelpServicesRepository IHelpServicesRepository;
        private readonly ILogger ILogger;
        public HelpServiceController(IMapper mapper, ILogger ILogger, IHelpServicesRepository IHelpServicesRepository)
        {
            this.IHelpServicesRepository = IHelpServicesRepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IHelpServicesRepository.GetAll();
            return Ok(Result);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save(HelpService HelpService)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            var Result = await this.IHelpServicesRepository.Save(HelpService);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item1);
        }

     
    }
}
