using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.API.Utilities;
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
    [Route("api/SystemSettings")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SsyetmSettingsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISystemSettingsRepository ISystemSettingsRepository;
        private readonly ILogger ILogger;
        public SsyetmSettingsController(IMapper mapper, ILogger ILogger, ISystemSettingsRepository ISystemSettingsRepository)
        {
            this.ISystemSettingsRepository = ISystemSettingsRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(SystemSetting SystemSettings)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            var Result = await ISystemSettingsRepository.Update(SystemSettings);
            if (!Result.Item1)
                return BadRequest();
            else
            {

                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = Convert.ToInt32(Operations.Update),
                    Page = "Latest News",
                    PageAr = "اخر الاخبار",
                    Details = ("System Settings  (" + SystemSettings.Id + ") is updated"),
                    DetailsAr = ("تم  تعديل  خصائص النظام (" + SystemSettings.Id + ")"),
                    Date = DateTime.Now
                });
                return Ok(Result.Item2);

            }
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.ISystemSettingsRepository.GetAll();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.ISystemSettingsRepository.GetById(Id);
            return Ok(Result);
        }
    }
}
