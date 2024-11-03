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
using Spectre.Core.Repositories;

namespace Spectre.API.Controllers
{

    [Route("api/Public")]
    [ApiController]
    public class PublicController : ControllerBase
    {

        private readonly IPackageRepository IPackageRepository;
        private readonly IHomePageReposiotory IHomePageReposiotory;
        private readonly ILatestNewsRepository ILatestNewsRepository;
        private readonly IContactUsRepository IContactUsRepository;
        private readonly ICountryRepository ICountryRepository;
        private readonly ICompanyPreRegistrationRepository ICompanyPreRegistrationRepository;
        private readonly IFeatureRepository IFeatureRepository;
        private readonly ILogger ILogger;
        public PublicController(IFeatureRepository IFeatureRepository, ICompanyPreRegistrationRepository ICompanyPreRegistrationRepository, ICountryRepository ICountryRepository, IContactUsRepository IContactUsRepository, ILatestNewsRepository ILatestNewsRepository, IHomePageReposiotory IHomePageReposiotory, ILogger ILogger, IPackageRepository IPackageRepository)
        {
            this.IPackageRepository = IPackageRepository;
            this.ILogger = ILogger;
            this.IHomePageReposiotory = IHomePageReposiotory;
            this.ILatestNewsRepository = ILatestNewsRepository;
            this.IContactUsRepository = IContactUsRepository;
            this.ICountryRepository = ICountryRepository;
            this.ICompanyPreRegistrationRepository = ICompanyPreRegistrationRepository;
            this.IFeatureRepository = IFeatureRepository;
        }

        [HttpPost]
        [Route("SendRegistration")]
        public async Task<IActionResult> Create(CompanyPreRegistration CompanyPreRegistration)
        {
            CompanyPreRegistration.CreationDate = DateTime.Now;
            var Result = await ICompanyPreRegistrationRepository.Create(CompanyPreRegistration);
            if (!Result.Item1)
                return BadRequest();

            return Ok(true);

        }



        [HttpGet]
        [Route("GetAllPackages")]
        public async Task<IActionResult> GetAllPackages()
        {
            var Result = await IPackageRepository.GetAll();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);

        }

        [HttpGet]
        [Route("GetWelcome")]
        public async Task<IActionResult> GetWelcome()
        {
            var Result = await IHomePageReposiotory.GetWelcome();
            return Ok(Result);

        }


        [HttpGet]
        [Route("GetHomePageContect")]
        public async Task<IActionResult> GetHomePageContect()
        {
            HomePageContet_View result = new HomePageContet_View();
            var _services = await IHomePageReposiotory.GetServices();
            var _aboutUs = await IHomePageReposiotory.GetAbout();
            var _visualizing = await IHomePageReposiotory.GetVisualize();
            var _snaps = await IHomePageReposiotory.GetReportSnaps();
            var _packages = await IPackageRepository.GetVisiblePackags();
            var _news = await ILatestNewsRepository.GetPublishedNews();
            var _country = await ICountryRepository.GetAll();
            var _footer = await IHomePageReposiotory.GetFooter();
            var _features = await IFeatureRepository.GetAll();

            result.Services = _services.ToList();
            result.AboutUs = _aboutUs.ToList();
            result.Visualizing = _visualizing.ToList();
            result.ReportSnaps = _snaps.ToList();
            result.Packages = _packages.ToList();
            result.LatestNews = _news.ToList();
            result.Countries = _country.ToList();
            result.Features = _features.Item2.ToList();
            result.SpectreFooter = _footer.ToList();

            return Ok(result);

        }

        [HttpGet]
        [Route("GetServices")]
        public async Task<IActionResult> GetServices()
        {
            var Result = await IHomePageReposiotory.GetServices();
            return Ok(Result);
        }

        [HttpGet]
        [Route("GetFooter")]
        public async Task<IActionResult> GetFooter()
        {
            var Result = await IHomePageReposiotory.GetFooter();
            return Ok(Result);
        }

        [HttpPost]
        [Route("SaveContactUs")]
        public async Task<IActionResult> SaveContactUs(ContactU contact)
        {
            var Result = await IContactUsRepository.Create(contact);
            return Ok(Result);

        }
    }
}
