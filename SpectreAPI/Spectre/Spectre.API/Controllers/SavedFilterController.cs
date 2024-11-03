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
    [Route("api/UserFilters")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SavedFilterController : ControllerBase
    {

        private readonly ISavedFilterRepository ISavedFilterRepository;
        private readonly ILogger ILogger;

        public SavedFilterController(ILogger ILogger, ISavedFilterRepository ISavedFilterRepository)
        {
            this.ISavedFilterRepository = ISavedFilterRepository;
            this.ILogger = ILogger;
        }

        [HttpPost]
        [Route("SaveUserFilter")]
        public async Task<IActionResult> SaveUserFilter(List<SavedFilters_View> data)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            foreach (SavedFilters_View item in data)
            {
                item.UserId = UserId;
            }

            var Result = await ISavedFilterRepository.SaveUserFilters(data);
            if (!Result.Item1)
                return BadRequest();
            return Ok(true);
        }

        [HttpGet]
        [Route("GetUserFilters")]
        public async Task<IActionResult> GetUserFilters(string PageUrl)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

           
            var Result = await ISavedFilterRepository.GetUsersSavedFilters(PageUrl, UserId);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }

        [HttpGet]
        [Route("GetDefault")]
        public async Task<IActionResult> GetDefault()
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;


            var Result = await ISavedFilterRepository.GetUserDefaultSettings(UserId);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }
    }
}
