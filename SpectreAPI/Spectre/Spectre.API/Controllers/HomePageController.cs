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
    [Route("api/HomePage")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HomePageController : ControllerBase
    {
        private readonly IHomePageReposiotory IHomePageReposiotory;
        private readonly ILogger ILogger;
        public HomePageController(ILogger ILogger, IHomePageReposiotory IHomePageReposiotory)
        {
            this.IHomePageReposiotory = IHomePageReposiotory;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAboutUs")]
        public async Task<IActionResult> GetAboutUs()
        {
            var Result = await this.IHomePageReposiotory.GetAbout();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdateAbout")]
        public async Task<IActionResult> UpdateAbout(AboutSpectre about)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateAbout(about);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "About Spectre",
                PageAr = "حول سبيكتر",
                Details = ("Updated about section"),
                DetailsAr = ("تم اضافة  رابطتم تحديث القسم"),
                Date = DateTime.Now
            });
            return Ok(true);
        }


        [HttpGet]
        [Route("GetServices")]
        public async Task<IActionResult> GetServices()
        {
            var Result = await this.IHomePageReposiotory.GetServices();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdatedServices")]
        public async Task<IActionResult> UpdatedServices(Service service)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateService(service);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Services",
                PageAr = "خدمات",
                Details = ("Updated service " + service.TitleEn),
                DetailsAr = ("تعديل خدمة " + service.TitleAr),
                Date = DateTime.Now
            });
            return Ok(true);
        }


        [HttpGet]
        [Route("GetVisualizing")]
        public async Task<IActionResult> GetVisualizing()
        {
            var Result = await this.IHomePageReposiotory.GetVisualize();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdateVisualizing")]
        public async Task<IActionResult> UpdateVisualizing(VisualizingReport report)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateVisualize(report);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Visualizing Reports",
                PageAr = "تصور التقارير",
                Details = ("Updated reports " + report.TitleEn),
                DetailsAr = ("تعديل تقارير " + report.TitleAr),
                Date = DateTime.Now
            });
            return Ok(true);
        }

        [HttpGet]
        [Route("GetWelcome")]
        public async Task<IActionResult> GetWelcome()
        {
            var Result = await this.IHomePageReposiotory.GetWelcome();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdateWelcome")]
        public async Task<IActionResult> UpdateWelcome(HomePageHeader welcome)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateWelcome(welcome);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Welcome",
                PageAr = "الترحيب",
                Details = "Updated Welcome note",
                DetailsAr = "تعديل الترحيب",
                Date = DateTime.Now
            });
            return Ok(true);
        }

        [HttpGet]
        [Route("GetReportSnaps")]
        public async Task<IActionResult> GetReportSnaps()
        {
            var Result = await this.IHomePageReposiotory.GetReportSnaps();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdateSnaps")]
        public async Task<IActionResult> UpdateSnaps(ReportSnap snap)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateSnap(snap);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Report Snaps",
                PageAr = "لقطات من التقارير",
                Details = "Report snap " + snap.ReferenceText + " has been updated",
                DetailsAr = "تم تعديل لقطات من التقارير   " + snap.ReferenceText ,
                Date = DateTime.Now
            });
            return Ok(true);
        }

        [HttpPost]
        [Route("SaveSnap")]
        public async Task<IActionResult> SaveSnap(ReportSnap snap)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.Save(snap);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Report Snaps",
                PageAr = "لقطات من التقارير",
                Details = "Report snap " + snap.ReferenceText + " has been updated",
                DetailsAr = "تم تعديل لقطات من التقارير   " + snap.ReferenceText,
                Date = DateTime.Now
            });
            return Ok(true);
        }


        [HttpPost]
        [Route("DeleteSnap")]
        public async Task<IActionResult> DeleteSnap(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.DeleteSnap(Id);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Delete),
                Page = "Report Snaps",
                PageAr = "لقطات من التقارير",
                Details = "Report snap " + Result.Item2.ReferenceText + " has been deleted",
                DetailsAr = "تم مسح لقطات من التقارير   " + Result.Item2.ReferenceText,
                Date = DateTime.Now
            });
            return Ok(true);
        }

        [HttpGet]
        [Route("GetFooter")]
        public async Task<IActionResult> GetFooter()
        {
            var Result = await this.IHomePageReposiotory.GetFooter();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdateFooter")]
        public async Task<IActionResult> UpdateFooter(SpectreFooter SpectreFooter)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateFooter(SpectreFooter);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Company Information",
                PageAr = "معلومات الشركة",
                Details = "Updated Company Information",
                DetailsAr = "تم تحديث معلومات الشركة ",
                Date = DateTime.Now
            });
            return Ok(true);
        }

        [HttpGet]
        [Route("GetHelpAboutUs")]
        public async Task<IActionResult> GetHelpAboutUs()
        {
            var Result = await this.IHomePageReposiotory.GetHelpAbout();
            return Ok(Result);
        }

        [HttpPost]
        [Route("UpdateHelpAbout")]
        public async Task<IActionResult> UpdateHelpAbout(HelpAboutU about)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IHomePageReposiotory.UpdateHelpAbout(about);
            if (!Result.Item1)
                return Ok(BadRequest());

            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Update),
                Page = "Help About Spectre",
                PageAr = "حول سبيكتر",
                Details = ("Updated about section"),
                DetailsAr = ("تم اضافة  رابطتم تحديث القسم"),
                Date = DateTime.Now
            });
            return Ok(true);
        }

    }
}
