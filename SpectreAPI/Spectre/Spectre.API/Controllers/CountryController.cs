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


namespace Spectre.API.Controllers
{
    [Route("api/Country")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class CountryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICountryRepository ICountryRepository;
        private readonly ILogger ILogger;
        public CountryController(IMapper mapper, ILogger ILogger, ICountryRepository ICountryRepository)
        {
            this.ICountryRepository = ICountryRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.ICountryRepository.GetAll();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.ICountryRepository.GetById(Id);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetByCode")]
        public async Task<IActionResult> GetByCode(string Code)
        {

            var Result = await this.ICountryRepository.GetByCode(Code);
            return Ok(Result);
        }


        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> Import(List<Country> Countries)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            foreach (Country Country in Countries)
            {
                var res = await ICountryRepository.Create(Country);


            }
            return Ok("Success");
        }

        [HttpGet]
        [Route("GetUserCountries")]
        public async Task<IActionResult> GetUserCountries(string PageUrl, string Source, string Lang)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.ICountryRepository.GetUserCountries(UserId, PageUrl, Source, Lang);
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);
        }
    }
}
