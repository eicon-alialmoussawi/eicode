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
namespace Spectre.API.Controllers
{
    [Route("api/RegisterationRequest")]
    [ApiController]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RegisterationRequestController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IRegisterationRequestRepository IRegisterationRequestRepository;
        private readonly ILogger ILogger;
        public RegisterationRequestController(IMapper mapper, ILogger ILogger, IRegisterationRequestRepository IRegisterationRequestRepository)
        {
            this.IRegisterationRequestRepository = IRegisterationRequestRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(RegisterationRequest RegisterationRequest)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            var Result = await IRegisterationRequestRepository.Create(RegisterationRequest);
            if (!Result.Item1)
                return BadRequest();
            else
            {
                if (RegisterationRequest.Id == 0)
                {
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),

                        Page = "RegisterationRequest",
                        PageAr = "طلبات التسجيل",
                        Details = ("Registeration Request (" + RegisterationRequest.Name + ") is added"),
                        DetailsAr = ("تم اضافة طلب تسجيل (" + RegisterationRequest.Name + ")"),
                        Date = DateTime.Now
                    });

                }
                else
                {
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),

                        Page = "RegisterationRequest",
                        PageAr = "طلبات التسجيل",
                        Details = ("Registeration Request (" + RegisterationRequest.Name + ") is Updated"),
                        DetailsAr = ("تم تعديل  طلب تسجيل (" + RegisterationRequest.Name + ")"),
                        Date = DateTime.Now
                    });

                }

                return Ok(Result.Item2);
            }
        }
    }
}
