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
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Spectre.API.Controllers
{
    [Route("api/Package")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PackageController : ControllerBase
    {

        private readonly IPackageRepository IPackageRepository;
        private readonly ILogger ILogger;
        public PackageController(ILogger ILogger, IPackageRepository IPackageRepository)
        {
            this.IPackageRepository = IPackageRepository;
            this.ILogger = ILogger;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Result = await IPackageRepository.GetAll();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);

        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            var Result = await IPackageRepository.GetById(Id);

            if (!Result.Item1)
                return Ok(BadRequest());
            var permissions = await IPackageRepository.GetPermissionByPackageId(Id);
            Package_View packageView = new Package_View();
            packageView.Id = Result.Item2.Id;
            packageView.NameEn = Result.Item2.NameEn;
            packageView.NameFr = Result.Item2.NameFr;
            packageView.NameSpa = Result.Item2.NameSpa;
            packageView.DescriptionEn = Result.Item2.DescriptionEn;
            packageView.DescriptionFr = Result.Item2.DescriptionFr;
            packageView.DescriptionSpa = Result.Item2.DescriptionSpa;
            packageView.Order = Result.Item2.Order;
            packageView.IsDemoPackage = Result.Item2.IsDemoPackage;
            packageView.IsVisible = Result.Item2.IsVisible;
            packageView.IsDeleted = Result.Item2.IsDeleted;
            packageView.ImageUrl = Result.Item2.ImageUrl;
            packageView.FromYearLimit = Result.Item2.FromYearLimit;
            packageView.ToYearLimit = Result.Item2.ToYearLimit;
            packageView.PagePermissions = permissions.Item2;


            return Ok(packageView);

        }

        [HttpPost]
        [Route("DeletePackage")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await IPackageRepository.DeletePackage(Id);
            if (!Result.Item1)
                return Ok(BadRequest());
            if (Result.Item2)
            {
                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = Convert.ToInt32(Operations.Delete),
                    Page = "Packages",
                    PageAr = "الحزم",
                    Details = ("Package(" + Id + ") is Deleted"),
                    DetailsAr = ("تم مسح  الحزم (" + Id + ")"),
                    Date = DateTime.Now
                });
            }
            return Ok(Result.Item2);
        }

        [HttpPost]
        [Route("SavePackage")]
        public async Task<IActionResult> SavePackage(Package_View package)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            package.FromYearLimit = package.FromYearLimit == null ? 0 : package.FromYearLimit;
            package.ToYearLimit = package.ToYearLimit == null ? 0 : package.ToYearLimit;

            var Result = await IPackageRepository.SavePackage(package);
            if (!Result.Item1)
                return Ok(BadRequest());
            if (Result.Item2.Success)
            {
                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = package.Id == 0 ? Convert.ToInt32(Operations.Add) : Convert.ToInt32(Operations.Update),
                    Page = "Packages",
                    PageAr = "الحزم",
                    Details = ("Package (" + package.NameEn + ") is " + (package.Id == 0 ? "added" : "updated")),
                    DetailsAr = ("تم " + (package.Id == 0 ? "اضافة" : "تعديل") + "  الحزم (" + package.NameEn + ")"),
                    Date = DateTime.Now
                });
            }
            return Ok(Result.Item2);
        }

        [HttpGet]
        [Route("GetPackagePermissions")]
        public async Task<IActionResult> GetPackagePermissions(int Id)
        {

            var permissions = await IPackageRepository.GetPackagePagePermissionDetails(Id);

            return Ok(permissions.Item2);

        }


    }
}
