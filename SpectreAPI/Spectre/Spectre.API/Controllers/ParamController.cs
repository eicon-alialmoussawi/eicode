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

    [Route("api/Params")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ParamController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IParamRepository IParamRepository;
        private readonly ILogger ILogger;
        public ParamController(IMapper mapper, ILogger ILogger, IParamRepository IParamRepository)
        {
            this.IParamRepository = IParamRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Parameters = await IParamRepository.GetAll();
            if (!Parameters.Item1)
                return BadRequest();
            return Ok(Parameters.Item2);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(int ParamId, string Value)
        {
            var TokenClaims = HttpContext.User;
            bool _IsAdmin = Convert.ToBoolean(TokenClaims.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            if (!_IsAdmin)
                return Ok(new ResponseMessage { Success = false, MessageEn = "You should be admin to apply this action", MessageAr = "يجب ان تكون مدير لانجاز هذا الاجراء" });

            var Parameters = await IParamRepository.Update(ParamId, Value);
            if (!Parameters.Item1)
                return BadRequest();
            else
            {
                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = Convert.ToInt32(Operations.Update),
                    Page = "Parameters",
                    PageAr = "المعلمات",
                    Details = ("Parameter is updated"),
                    DetailsAr = ("تم تعديل  المعلمات "),
                    Date = DateTime.Now
                });
            }
            return Ok(Parameters.Item2);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            var Parameters = await IParamRepository.GetById(Id);
            if (!Parameters.Item1)
                return BadRequest();
            return Ok(Parameters.Item2);
        }
    }
}
