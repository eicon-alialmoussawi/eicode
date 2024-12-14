using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models.Extenders;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/BazAwards")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BazAwardController : Controller
    {
        private readonly IBazAwardRepository IBazAwardRepository;
        public BazAwardController(IBazAwardRepository bazAwardRepository)
        {
            IBazAwardRepository = bazAwardRepository;

        }
        [HttpPost]
        [Route("GetFilteredBazAwards")]
        public async Task<IActionResult> GetFilteredBazAwards(BazAwardFilter_View view)
        {
            var Result = await this.IBazAwardRepository.GetFilteredBazAwards(view);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IBazAwardRepository.GetAll();
            return Ok(Result);
        }
    }
}
