using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/Permission")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PermissionController : Controller
    {

        private readonly IMapper mapper;
        private readonly IPermissionRepository IPermissionRepository;

        public PermissionController(IMapper mapper, IPermissionRepository IPermissionRepository)
        {
            this.IPermissionRepository = IPermissionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        [Route("GetUserPermissions")]
        public async Task<IActionResult> GetUserPermissions(string PageUrl)
        {
            var TokenClaims = HttpContext.User;
            string IdentityUserId = TokenClaims.Claims.FirstOrDefault(c => c.Type == "IdentityUserId").Value.ToString();

            var Permissions = await IPermissionRepository.GetUserPermissions(IdentityUserId, "", PageUrl);
            if (!Permissions.Item1)
                return BadRequest();
            return Ok(Permissions.Item2);
        }

        [HttpGet]
        [Route("GetMenuPermissions")]
        public async Task<IActionResult> GetMenuPermissions()
        {
            var TokenClaims = HttpContext.User;
            string IdentityUserId = TokenClaims.Claims.FirstOrDefault(c => c.Type == "IdentityUserId").Value.ToString();

            var Permissions = await IPermissionRepository.GetUserPermissions(IdentityUserId, "", "");
            if (!Permissions.Item1)
                return BadRequest();
            return Ok(Permissions.Item2);
        }



        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Permissions = await IPermissionRepository.GetAllPermissions();
            if (!Permissions.Item1)
                return BadRequest();
            return Ok(Permissions.Item2);
        }

        [HttpGet]
        [Route("CheckIfUserCanView")]
        public async Task<IActionResult> CheckIfUserCanView(string PageUrl )
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Permissions = await IPermissionRepository.CheckIfUserCanView(UserId, PageUrl);
            if (!Permissions.Item1)
                return BadRequest();
            return Ok(Permissions.Item2);
        }



    }
}
