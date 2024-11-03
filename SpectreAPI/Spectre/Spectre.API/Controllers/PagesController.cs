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
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace Spectre.API.Controllers
{

    [Route("api/Pages")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PagesController : ControllerBase
    {
        private readonly IPageRepository IPageRepository;
        private readonly ILogger ILogger;
        public PagesController(ILogger ILogger, IPageRepository IPageRepository)
        {
            this.IPageRepository = IPageRepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Result = await IPageRepository.GetAll();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);

        }

    }
}
