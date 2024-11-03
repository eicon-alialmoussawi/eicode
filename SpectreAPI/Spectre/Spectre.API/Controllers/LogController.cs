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
    [Route("api/Logs")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LogController : ControllerBase
    {
        private readonly ILogger ILogger;
        public LogController(ILogger ILogger)
        {
            this.ILogger = ILogger;
        }


        [HttpGet]
        [Route("GetExceptions")]
        public async Task<IActionResult> GetExceptions()
        {

            var Result = await this.ILogger.GetExceptionLogs();
            return Ok(Result);
        }
    }
}
