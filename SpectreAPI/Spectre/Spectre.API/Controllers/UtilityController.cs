
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Spectre.API.Controllers
{
    [Route("api/Utility")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UtilityController : ControllerBase
    {
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public UtilityController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("UploadFile")]
        [Obsolete]
        public async Task<IActionResult> UploadFile(IFormFile File)
        {
                var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Media");

            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            string FilePath = "";
            string NewFileName = "";
            if (File.Length > 0)
            {
                string Ext = Path.GetExtension(File.FileName.Trim());
                NewFileName = Guid.NewGuid() + Ext;
                FilePath = Path.Combine(uploads, NewFileName);
                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    await File.CopyToAsync(fileStream);
                }
            }

            return Ok(NewFileName);
        }


    }
}
