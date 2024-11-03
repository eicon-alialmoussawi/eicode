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


namespace Spectre.API.Controllers
{
    [Route("api/Icon")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IconController : ControllerBase
    {
        private readonly IIconRepository IIconRepository;
        private readonly ILogger ILogger;
        public IconController(ILogger ILogger, IIconRepository IIconRepository)
        {
            this.IIconRepository = IIconRepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Result = await this.IIconRepository.GetAll();
            return Ok(Result);
        }



    }
}
