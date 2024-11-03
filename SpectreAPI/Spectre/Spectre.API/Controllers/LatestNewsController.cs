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
    [Route("api/LatestNews")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LatestNewsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ILatestNewsRepository ILatestNewsRepository;
        private readonly ILogger ILogger;
        public LatestNewsController(IMapper mapper, ILogger ILogger, ILatestNewsRepository ILatestNewsRepository)
        {
            this.ILatestNewsRepository = ILatestNewsRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(LatestNews LatestNews)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            if (LatestNews.Id == 0)
            {

                LatestNews.CreationDate = DateTime.Now;
                var Result = await ILatestNewsRepository.Create(LatestNews);
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
                        Details = ("Latest News  (" + LatestNews.DescriptionEn + ") is added"),
                        DetailsAr = ("تم  اضافة اخر الاخبار (" + LatestNews.DescriptionAr + ")"),
                        Date = DateTime.Now
                    });
                    return Ok(Result.Item2);
                }
            }
            else
            {
                var Result = await ILatestNewsRepository.Update(LatestNews);
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
                        Details = ("Latest News  (" + LatestNews.DescriptionEn + ") is updated"),
                        DetailsAr = ("تم  تعديل اخر الاخبار (" + LatestNews.DescriptionAr + ")"),
                        Date = DateTime.Now
                    });
                    return Ok(Result.Item2);
                }

            }
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.ILatestNewsRepository.GetAll();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.ILatestNewsRepository.GetById(Id);
            return Ok(Result);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            var Result = await this.ILatestNewsRepository.GetById(Id);
            Result.IsDeleted = true;
            var Res = await ILatestNewsRepository.Update(Result);
            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Delete),
                Page = "Latest News",
                PageAr = "اخر الاخبار",
                Details = ("Latest News  for Year(" + Result.DescriptionEn + ") is Deleted"),
                DetailsAr = ("تم  مسح اخر الاخبار (" + Result.DescriptionAr + ")"),
                Date = DateTime.Now
            });
            return Ok(Result);
        }

    }
}
