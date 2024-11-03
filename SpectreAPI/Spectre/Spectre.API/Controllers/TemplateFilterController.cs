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
    [Route("api/TemplateFilter")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TemplateFilterController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ILogger ILogger;
        public readonly ITemplateFilterRepository ITemplateFilterRepository;
        public readonly ILookupRepository ILookupRepository;
        public TemplateFilterController(IMapper mapper, ILogger ILogger, ITemplateFilterRepository ITemplateFilterRepository, ILookupRepository ILookupRepository)
        {
            this.ILookupRepository = ILookupRepository;
            this.ITemplateFilterRepository = ITemplateFilterRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }


        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(TemplateFilter TemplateFilter)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            if (TemplateFilter.Id == 0)
            {
                TemplateFilter.CreationDate = DateTime.Now;
                TemplateFilter.CreatedBy = UserId;
                var Result = await ITemplateFilterRepository.Create(TemplateFilter);
                if (!Result.Item1)
                    return BadRequest();
                else
                {

                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),

                        Page = "TemplateFilter",
                        PageAr = "فلتر",
                        Details = ("TemplateFilter is added"),
                        DetailsAr = ("تم اضافة فلتر"),
                        Date = DateTime.Now
                    });



                    return Ok(Result.Item2);
                }
            }
            else
            {

                var Result = await ITemplateFilterRepository.Update(TemplateFilter);

                if (!Result.Item1)
                    return BadRequest();
                else
                {
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),

                        Page = "TemplateFilter",
                        PageAr = "فلتر",
                        Details = ("TemplateFilter is Updated"),
                        DetailsAr = ("تم تعديل فلتر"),
                        Date = DateTime.Now
                    });

                }

                return Ok(Result.Item2);
            }
        }


        [HttpPost]
        [Route("AddListFilterDetails")]
        public async Task<IActionResult> AddListFilterDetails(List<TemplateFilterDetail> TemplateFilterDetails)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            TemplateFilter TempFilter = new TemplateFilter();
            TempFilter.CreatedBy = UserId;
            TempFilter.CreationDate = DateTime.Now;
            TempFilter.IsDeleted = false;
            TempFilter.PageName = "AwardsMenu";
            TempFilter.Title = DateTime.Now.ToString();
            var res = await ITemplateFilterRepository.Create(TempFilter);
            foreach (TemplateFilterDetail TemplateFilterDetail in TemplateFilterDetails)
            {
                TemplateFilterDetail.CreationDate = DateTime.Now;
                TemplateFilterDetail.CreatedBy = UserId;
                TemplateFilterDetail.TemplateId = res.Item2.Id;
                TemplateFilterDetail.IsDeleted = false;
                var Result = await ITemplateFilterRepository.AddFilterDetails(TemplateFilterDetail);
            }
            return Ok(true);

        }


        [HttpPost]
        [Route("addFilterPricing")]
        public async Task<IActionResult> addFilterPricing(List<TemplateFilterDetail> TemplateFilterDetails)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            TemplateFilter TempFilter = new TemplateFilter();
            TempFilter.CreatedBy = UserId;
            TempFilter.CreationDate = DateTime.Now;
            TempFilter.IsDeleted = false;
            TempFilter.PageName = "Pricing";
            TempFilter.Title = DateTime.Now.ToString();
            var res = await ITemplateFilterRepository.Create(TempFilter);
            foreach (TemplateFilterDetail TemplateFilterDetail in TemplateFilterDetails)
            {
                TemplateFilterDetail.CreationDate = DateTime.Now;
                TemplateFilterDetail.CreatedBy = UserId;
                TemplateFilterDetail.TemplateId = res.Item2.Id;
                TemplateFilterDetail.IsDeleted = false;
                var Result = await ITemplateFilterRepository.AddFilterDetails(TemplateFilterDetail);
            }
            return Ok(true);

        }

        
        [HttpPost]
        [Route("AddFilterDetails")]
        public async Task<IActionResult> AddFilterDetails(TemplateFilterDetail TemplateFilterDetail)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            TemplateFilterDetail.CreationDate = DateTime.Now;
            TemplateFilterDetail.CreatedBy = UserId;
            var Result = await ITemplateFilterRepository.AddFilterDetails(TemplateFilterDetail);
            if (!Result.Item1)
                return BadRequest();
            else
            {

                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = Convert.ToInt32(Operations.Update),

                    Page = "TemplateFilterDetail",
                    PageAr = "تفاصيل الفلتر",
                    Details = ("Template Filter Details is added"),
                    DetailsAr = ("تم اضافة تفاصيل فلتر"),
                    Date = DateTime.Now
                });



                return Ok(Result.Item2);
            }


        }


        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.ITemplateFilterRepository.GetAll();
            return Ok(Result);
        }


        [HttpGet]
        [Route("GetAllByPage")]
        public async Task<IActionResult> GetAllByPage(string Page)
        {

            var Result = await this.ITemplateFilterRepository.GetAllByPage(Page);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetDetailsByFilterId")]
        public async Task<IActionResult> GetDetailsByFilterId(int Id)
        {

            var Result = await this.ITemplateFilterRepository.GetDetailsByFilterId(Id);
            return Ok(Result);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            var Result = await this.ITemplateFilterRepository.GetById(Id);
            Result.IsDeleted = true;
            var Res = await ITemplateFilterRepository.Update(Result);
            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Delete),
                Page = "TemplateFilter",
                PageAr = "فلتر",
                Details = ("TemplateFilter is Deleted"),
                DetailsAr = ("تم مسح فلتر"),
                Date = DateTime.Now
            });
            return Ok(Result);
        }

    }
}
