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
using Microsoft.AspNetCore.Cors;

namespace Spectre.API.Controllers
{
    [Route("api/Bands")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[EnableCors("MyPolicy")]
    public class BandsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBandRepository IBandRepository;
        private readonly ILogger ILogger;
        public BandsController(IMapper mapper, ILogger ILogger, IBandRepository IBandRepository)
        {
            this.IBandRepository = IBandRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Band Band)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            if (Band.Id == 0)
            {
                Band.CreationDate = DateTime.Now;
                Band.CreatedBy = UserId;
                var Result = await IBandRepository.Create(Band);
                if (!Result.Item1)
                    return BadRequest();
                else
                {

                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),

                        Page = "Band",
                        PageAr = "رابط",
                        Details = ("Band  for Year(" + Band.Value + ") is added"),
                        DetailsAr = ("تم اضافة  رابط (" + Band.Value + ")"),
                        Date = DateTime.Now
                    });



                    return Ok(Result.Item2);
                }
            }
            else
            {
                var res =await IBandRepository.GetById(Band.Id);
                Band.CreationDate = res.CreationDate;
                Band.IsDeleted = res.IsDeleted;
                Band.CreatedBy = res.CreatedBy;
                var Result = await IBandRepository.Update(Band);

                if (!Result.Item1)
                    return BadRequest();
                else
                {
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),
                        Page = "Band",
                        PageAr = "رابط",
                        Details = ("Band  for Year(" + Band.Value + ") is updated"),
                        DetailsAr = ("تم تعديل  رابط (" + Band.Value + ")"),
                        Date = DateTime.Now
                    });

                }

                return Ok(Result.Item2);
            }
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IBandRepository.GetAll();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.IBandRepository.GetById(Id);
            return Ok(Result);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            var Result = await this.IBandRepository.GetById(Id);
            Result.IsDeleted = true;
            var Res = await IBandRepository.Update(Result);
            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Delete),
                Page = "Award",
                PageAr = "جائزة",
                Details = ("Band  for Year(" + Result.Value + ") is Deleted"),
                DetailsAr = ("تم مسح  رابط (" + Result.Value + ")"),
                Date = DateTime.Now
            });
            return Ok(Result);
        }


        [HttpPost]
        [Route("SaveBand")]
        public async Task<IActionResult> SaveBand(Band band)
        {

            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            band.CreatedBy = UserId;
            band.CreationDate = DateTime.Now;

            var Result = await this.IBandRepository.SaveBand(band);
            if (!Result.Item1)
                return BadRequest();

            return Ok(Result.Item2);
        }


        [HttpPost]
        [Route("RemoveAllBands")]
        public async Task<IActionResult> RemoveAllBands()
        {

            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;


            var Result = await this.IBandRepository.RemoveAllBands();
            if (!Result.Item1)
                return BadRequest();

            return Ok(Result.Item1);
        }


        [HttpPost]
        [Route("DeleteBandById")]
        public async Task<IActionResult> DeleteBandById(int Id)
        {

            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;


            var Result = await this.IBandRepository.DeleteBand(Id);
            if (!Result.Item1)
                return BadRequest();

            return Ok(Result.Item1);
        }

    }
}
