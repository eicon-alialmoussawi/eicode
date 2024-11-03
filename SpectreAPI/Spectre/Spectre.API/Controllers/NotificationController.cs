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
    [Route("api/Notifications")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly INotificationRepository INotificationRepository;
        private readonly ILogger ILogger;
        public NotificationController(IMapper mapper, ILogger ILogger, INotificationRepository INotificationRepository)
        {
            this.INotificationRepository = INotificationRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(string Status)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await INotificationRepository.GetUserNotifications(UserId, Status);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
                return Ok(Result.Item2);
        }

        [HttpPost]
        [Route("SetAsViewed")]
        public async Task<IActionResult> SetAsViewed(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await INotificationRepository.SetNotificationAsViewed(Id);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
                return Ok(Result.Item1);
        }

        [HttpPost]
        [Route("SendNotification")]
        public async Task<IActionResult> SendNotification(string TextEn, string TextAr, string TextFr)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await INotificationRepository.SendNotification(TextEn, TextAr, TextFr);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
                return Ok(Result.Item1);
        }

        [HttpPost]
        [Route("SetUserNotificationAsSeen")]
        public async Task<IActionResult> SetUserNotificationAsSeen()
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await INotificationRepository.SetUserNotificationAsSeen(UserId);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
                return Ok(Result.Item1);
        }

        [HttpGet]
        [Route("GetUnSeenNotifications")]
        public async Task<IActionResult> GetUnSeenNotifications()
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await INotificationRepository.GetUnSeenNotifications(UserId);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
                return Ok(Result.Item2);
        }

    }
}
