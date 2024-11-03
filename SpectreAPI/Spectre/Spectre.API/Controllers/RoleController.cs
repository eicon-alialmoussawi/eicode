using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spectre.API.Utilities;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/Role")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoleController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly IRoleRepository IRoleRepository;
        private readonly ILogger ILogger;

        public RoleController(IMapper mapper, IRoleRepository IRoleRepository, UserManager<IdentityUser> UserManager, RoleManager<IdentityRole> RoleManager, ILogger ILogger)
        {
            this.mapper = mapper;
            this.IRoleRepository = IRoleRepository;
            this.RoleManager = RoleManager;
            this.mapper = mapper;
            this.ILogger = ILogger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] RoleResponse Role)
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            string IdentityUserId = TokenClaims.Claims.FirstOrDefault(c => c.Type == "IdentityUserId").Value.ToString();
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            bool _IsAdmin = Convert.ToBoolean(TokenClaims.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!_IsAdmin)
                return Ok(new ResponseMessage { Success = false, MessageEn = "You should be admin to apply this action", MessageAr = "يجب ان تكون مدير لانجاز هذا الاجراء" });

            bool RoleExist = await RoleManager.RoleExistsAsync(Role.RoleName);
            if (RoleExist && (Role.Id == null))
            {
                return Ok(new ResponseMessage { Success = false, MessageEn = "Another role with the same name exists", MessageAr = "هناك دور اخر بنفس الاسم" });
            }
            else
            {
                if (Role.Id == null)
                {
                    IdentityResult RoleResult = await RoleManager.CreateAsync(new IdentityRole { Name = Role.RoleName });
                    if (!RoleResult.Succeeded)
                        return Ok(new ResponseMessage { Success = false, MessageEn = RoleResult.Errors.ToString() });
                    else
                    {
                        var NewRole = await RoleManager.FindByNameAsync(Role.RoleName);
                        var SaveRolePermissionsResult = await IRoleRepository.SaveRolePermissions(new Guid(NewRole.Id), Role.Permissions);
                        if (!SaveRolePermissionsResult.Item1)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "API encountered an error while saving the permissions", MessageAr = "حصل خطأ في النظام" });
                        else
                        {
                            await ILogger.LogUserAction(new UserActionLog
                            {
                                Id = 0,
                                UserId = UserId,
                                Action = Convert.ToInt32(Operations.Add),
                                Page = "Roles",
                                PageAr = "الادوار",
                                Details = ("Role " + NewRole.Name + " is added"),
                                DetailsAr = ("تم اضافة الدور " + NewRole.Name),
                                Date = DateTime.Now
                            });
                            return Ok(new ResponseMessage { Success = true, MessageEn = "" });
                        }
                    }
                }
                else
                {
                    var OldRole = await RoleManager.FindByIdAsync(Role.Id.ToString());
                    OldRole.Name = Role.RoleName;

                    IdentityResult RoleResult = await RoleManager.UpdateAsync(OldRole);
                    if (!RoleResult.Succeeded)
                        return Ok(new ResponseMessage { Success = false, MessageEn = RoleResult.Errors.ToString() });
                    else
                    {
                        var SaveRolePermissionsResult = await IRoleRepository.SaveRolePermissions((Guid)Role.Id, Role.Permissions);
                        if (!SaveRolePermissionsResult.Item1)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "API encountered an error while saving the permissions", MessageAr = "حصل خطأ في النظام" });
                        else
                        {
                            await ILogger.LogUserAction(new UserActionLog
                            {
                                Id = 0,
                                UserId = UserId,
                                Action = Convert.ToInt32(Operations.Update),
                                Page = "Roles",
                                PageAr = "الادوار",
                                Details = ("Role " + OldRole.Name + " is updated"),
                                DetailsAr = ("تم تعديل الدور " + OldRole.Name),
                                Date = DateTime.Now
                            });
                            return Ok(new ResponseMessage { Success = true, MessageEn = "" });

                        }
                    }

                }

            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Roles = RoleManager.Roles.ToList().OrderBy(c => c.ConcurrencyStamp);
            return Ok(Roles);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(string RoleId)
        {
            var Role = await IRoleRepository.GetRoleById(new Guid(RoleId));
            if (!Role.Item1)
                return BadRequest();
            return Ok(Role.Item2);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string RoleId)
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            string IdentityUserId = TokenClaims.Claims.FirstOrDefault(c => c.Type == "IdentityUserId").Value.ToString();
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            bool _IsAdmin = Convert.ToBoolean(TokenClaims.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!_IsAdmin)
                return Ok(new ResponseMessage { Success = false, MessageEn = "You should be admin to apply this action", MessageAr = "يجب ان تكون مدير لانجاز هذا الاجراء" });

            var Role = await RoleManager.FindByIdAsync(RoleId);
            if (Role != null)
            {
                if (Role.Id == "82736e4c-d61e-47b4-ac40-a8baabbcdd4c")
                {
                    return Ok(new ResponseMessage { Success = false, MessageEn = "Sorry, Client role cant be deleted", MessageAr = "عذرًا ، لا يمكن حذف دور المعلم" });
                }

                if (Role.Id == "7f7d7be3-e659-416b-b60b-a05648fe00ab")
                {
                    return Ok(new ResponseMessage { Success = false, MessageEn = "Sorry, Admin role cant be deleted", MessageAr = "عذرًا ، لا يمكن حذف دور المدير" });
                }

                IdentityResult Result = await RoleManager.DeleteAsync(Role);
                if (!Result.Succeeded)
                    return Ok(new ResponseMessage { Success = false, MessageEn = Result.Errors.ToString() });
                else
                {
                    await this.IRoleRepository.DeleteRolePermissions(RoleId);
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Delete),
                        Page = "Roles",
                        PageAr = "الادوار",
                        Details = ("Role " + Role.Name + " is deleted"),
                        DetailsAr = ("تم مسح الدور " + Role.Name),
                        Date = DateTime.Now
                    });
                    return Ok(new ResponseMessage { Success = true, MessageEn = "" });
                }
            }
            else
            {
                return Ok(new ResponseMessage { Success = false, MessageEn = "Role does not exist", MessageAr = "هذا الدور غير موجود" });

            }
        }


        [HttpGet]
        [Route("GetAllExceptClient")]
        public async Task<IActionResult> GetAllExceptClient()
        {
            var Roles = RoleManager.Roles.Where(x=> x.Id != "82736e4c-d61e-47b4-ac40-a8baabbcdd4c").ToList().OrderBy(c => c.ConcurrencyStamp);
            return Ok(Roles);
        }

    }
}
