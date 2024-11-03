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
    [Route("api/FQA")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FQAController : ControllerBase
    {
        private readonly IFQARepository IFQARepository;
        private readonly ILogger ILogger;
        public FQAController(IMapper mapper, ILogger ILogger, IFQARepository IFQARepository)
        {
            this.IFQARepository = IFQARepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IFQARepository.GetAll();
            return Ok(Result);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save(Fqa Fqa)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            var Result = await this.IFQARepository.Save(Fqa);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item1);
        }


        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            var Result = await this.IFQARepository.Delete(Id);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item1);
        }

    }
}
