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
    [Route("api/CompanyPreRegistration")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyPreRegistrationController : ControllerBase
    {
        private readonly ICompanyPreRegistrationRepository ICompanyPreRegistrationRepository;
        private readonly ILogger ILogger;
        public CompanyPreRegistrationController(ILogger ILogger, ICompanyPreRegistrationRepository ICompanyPreRegistrationRepository)
        {
            this.ICompanyPreRegistrationRepository = ICompanyPreRegistrationRepository;
            this.ILogger = ILogger;
        }

       

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var Result = await this.ICompanyPreRegistrationRepository.GetAll();
                return Ok(Result);
            }
            catch (Exception e)
            {
                return Ok(false);
            }

        }

        [HttpGet]
        [Route("GetWithDetails")]
        public async Task<IActionResult> GetWithDetails()
        {

            var Result = await this.ICompanyPreRegistrationRepository.GetAllDetails();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);
        }


        [HttpPost]
        [Route("SetAsViewed")]
        public async Task<IActionResult> SetAsViewed(int Id)
        {

            var Result = await this.ICompanyPreRegistrationRepository.SetPreRegistrationAsViewed(Id);
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item1);
        }


        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {

            var Result = await this.ICompanyPreRegistrationRepository.DeletePreRegistration(Id);
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item1);
        }


        [HttpGet]
        [Route("GetDetailsWithId")]
        public async Task<IActionResult> GetDetailsWithId(int Id)
        {

            var Result = await this.ICompanyPreRegistrationRepository.GetPreRegistrationDetailsById(Id);
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);
        }


    }
}
