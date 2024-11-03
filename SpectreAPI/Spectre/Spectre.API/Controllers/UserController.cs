using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spectre.API.Utilities;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Core.RepositoryHandler;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Spectre.API.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private string UserName;
        private readonly IMapper mapper;
        private readonly IUserRepository IUserRepository;
        private readonly UserManager<IdentityUser> UserManager;
        private readonly ILogger ILogger;
        private readonly IConfiguration Config;
        private readonly RoleManager<IdentityRole> RoleManager;
        public UserController(IConfiguration Config, IMapper mapper, IUserRepository IUserRepository, ILogger ILogger, UserManager<IdentityUser> UserManager, RoleManager<IdentityRole> RoleManager)
        {
            this.IUserRepository = IUserRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
            this.RoleManager = RoleManager;
            this.UserManager = UserManager;
            this.Config = Config;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] LoginUser LoginUser)
        {
            var options = UserManager.Options.Password;

            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            bool _IsAdmin = Convert.ToBoolean(TokenClaims.Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value);

            if (!_IsAdmin)
                return Ok(new ResponseMessage { Success = false, MessageEn = "You should be admin to apply this action", MessageAr = "يجب ان تكون مدير لانجاز هذا الاجراء" });

            if (ModelState.IsValid)
            {
                var AllUsers = await this.IUserRepository.GetAllExistingUsers();
                if (LoginUser.Id == 0)
                {
                    IdentityUser ExistUserName = await UserManager.FindByNameAsync(LoginUser.UserName);
                    if (ExistUserName != null)
                        return Ok(new ResponseMessage { Success = false, MessageEn = "Another user with the same username exists", MessageAr = "هناك مستخدم اخر بنفس اسم المستخدم" });

                    IdentityUser ExistEmail = await UserManager.FindByEmailAsync(LoginUser.Email);
                    if (ExistEmail != null)
                        return Ok(new ResponseMessage { Success = false, MessageEn = "Another user with the same email exists", MessageAr = "هناك مستخدم اخر بنفس البريد الالكتروني " });

                    IdentityUser _User = new IdentityUser
                    {
                        UserName = LoginUser.UserName,
                        Email = LoginUser.Email,
                        PhoneNumber = LoginUser.PhoneNumber,
                    };


                    IdentityResult IdentityResult = await UserManager.CreateAsync(_User, LoginUser.Password);
                    if (!IdentityResult.Succeeded)
                        return BadRequest(IdentityResult.Errors);
                    else
                    {
                        User User = new User();
                        //User.UserId = 0;
                        User.Id = 0;
                        User.Name = LoginUser.Name;
                        User.LastName = LoginUser.LastName;
                        User.UserName = LoginUser.UserName;
                        User.Email = LoginUser.Email;
                        User.PhoneNumber = LoginUser.PhoneNumber;
                        User.IdentityUserId = _User.Id;
                        User.CreationDate = DateTime.Now;
                        User.IsDeleted = false;
                        User.IsApproved = true;
                        User.IsAdmin = LoginUser.IsAdmin;
                        User.IsLocked = LoginUser.IsLocked;
                        User.UserType = "U_SYS";
                        User.JobTitle = LoginUser.JobTitle;

                        var UserResult = await this.IUserRepository.Create(User);
                        if (!UserResult.Item1)
                            return BadRequest();

                        if (LoginUser.RoleId != null)
                        {
                            var Role = await RoleManager.FindByIdAsync(LoginUser.RoleId);
                            await UserManager.AddToRoleAsync(_User, Role.Name);
                        }
                    }

                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Add),
                        Page = "Users",
                        PageAr = "المستخدمين",
                        Details = ("User (" + _User.UserName + ") is added"),
                        DetailsAr = ("تم اضافة المستخدم (" + _User.UserName + ")"),
                        Date = DateTime.Now
                    });


                    string Authority = Config.GetValue<string>("SettingKeys:WebURL");

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<div dir='ltr'>");
                    sb.Append("<p>");
                    sb.Append("Dear " + LoginUser.UserName + ",");
                    sb.Append("</p>");
                    sb.Append("<p>");
                    sb.Append("Please note that your account in Spectre has been created: ");
                    sb.Append("</p>");

                    sb.Append("Please find below your credentials: ");
                    sb.Append("<ol>");

                    sb.Append("<li>");
                    sb.Append("<strong>Username: </strong> " + LoginUser.UserName);
                    sb.Append("</li>");
                    sb.Append("<li>");
                    sb.Append("<strong>Password: </strong> " + LoginUser.Password);
                    sb.Append("</li>");
                    sb.Append("</ol>");

                    sb.Append("</br>");
                    sb.Append("<p>");
                    sb.Append("Kind regards,");
                    sb.Append("</p>");
                    sb.Append("The Administrator");
                    sb.Append("</br>");
                    sb.Append("</div>");

                    string To = LoginUser.Email;
                    string From = Config.GetValue<string>("SettingKeys:SendingEmail");
                    string SMTPUserName = Config.GetValue<string>("SettingKeys:SmtpServerUsername");
                    int Port = Config.GetValue<int>("SettingKeys:SmtpServerPort");
                    string SMPTPassword = Config.GetValue<string>("SettingKeys:SmtpServerPassword");
                    string HostServer = Config.GetValue<string>("SettingKeys:SmtpServer");
                   
                    if(!Helpers.SendEmail(To, sb.ToString(), From, "Account Creation", SMTPUserName, SMPTPassword, HostServer, Port))
                        return Ok(new ResponseMessage { Success = false, MessageEn = "Something went wrong when sending email" });

                    return Ok(new ResponseMessage { Success = true, MessageEn = "" });
                }
                else
                {
                    var MyUser = AllUsers.Item2.FirstOrDefault(c => c.Id == LoginUser.Id);
                    IdentityUser ThisUser = await UserManager.FindByIdAsync(MyUser.IdentityUserId);
                    if (ThisUser.Email != LoginUser.Email)
                    {
                        IdentityUser ExistEmail = await UserManager.FindByEmailAsync(LoginUser.Email);
                        if (ExistEmail != null)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "Another user with the same email exists", MessageAr = "هناك مستخدم اخر بنفس البريد الالكتروني" });
                    }

                    ThisUser.Email = LoginUser.Email;
                    ThisUser.PhoneNumber = LoginUser.PhoneNumber;
                    IdentityResult IdentityResult = await UserManager.UpdateAsync(ThisUser);
                    if (!IdentityResult.Succeeded)
                        return BadRequest(IdentityResult.Errors);

                    MyUser.Name = LoginUser.Name;
                    MyUser.LastName = LoginUser.LastName;
                    MyUser.UserName = MyUser.UserName;
                    MyUser.IdentityNumber = MyUser.IdentityNumber;
                    MyUser.IdentityUserId = ThisUser.Id;
                    MyUser.Email = LoginUser.Email;
                    MyUser.PhoneNumber = LoginUser.PhoneNumber;
                    MyUser.IsAdmin = LoginUser.IsAdmin;
                    MyUser.Dob = LoginUser.DOB;
                    MyUser.IsLocked = LoginUser.IsLocked;

                    var UserResult = await this.IUserRepository.Update(MyUser);
                    if (!UserResult.Item1)
                        return BadRequest();

                    if (LoginUser.RoleId != null)
                    {
                        var Role = await RoleManager.FindByIdAsync(LoginUser.RoleId);
                        var UserRoles = await UserManager.GetRolesAsync(ThisUser);
                        if (UserRoles.Count() > 0)
                        {
                            if (UserRoles.FirstOrDefault() != Role.Name)
                            {
                                await UserManager.RemoveFromRoleAsync(ThisUser, UserRoles.FirstOrDefault());
                                await UserManager.AddToRoleAsync(ThisUser, Role.Name);
                            }
                        }
                        else
                            await UserManager.AddToRoleAsync(ThisUser, Role.Name);
                    }
                    else
                    {
                        var UserRoles = await UserManager.GetRolesAsync(ThisUser);
                        if (UserRoles.Count() > 0)
                            await UserManager.RemoveFromRoleAsync(ThisUser, UserRoles.FirstOrDefault());
                    }

                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),
                        Page = "Users",
                        PageAr = "المستخدمين",
                        Details = ("User (" + ThisUser.UserName + ") is updated"),
                        DetailsAr = ("تم تعديل المستخدم (" + ThisUser.UserName + ")"),
                        Date = DateTime.Now
                    });
                    return Ok(new ResponseMessage { Success = true, MessageEn = "" });
                }
            }
            else
                return BadRequest("Invalid Model State");
        }
        [HttpGet]
        [Route("GetAllExistingUsers")]
        public async Task<IActionResult> GetAllExistingUsers()
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;

            var AllUsers = await this.IUserRepository.GetAllExistingUsers();
            if (!AllUsers.Item1)
                return BadRequest("Api encountered an error");

            return Ok(AllUsers.Item2);
        }


        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {

            var Result = await this.IUserRepository.GetById(Id);
            Result.Item2.IsDeleted = true;

            var UserResult = await this.IUserRepository.Create(Result.Item2);
            if (!UserResult.Item1)
                return BadRequest();
            return Ok(UserResult);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            //   var Result = await this.IUserRepository.GetById(Id);
            var TokenClaims = HttpContext.User;

            var User = await this.IUserRepository.GetById(Id);
            if (!User.Item1)
                return BadRequest("Api encountered an error");

            var IdentityUser = await UserManager.FindByIdAsync(User.Item2.IdentityUserId);
            var UserRoles = await UserManager.GetRolesAsync(IdentityUser);
            UserModel_1 UserResponse = new UserModel_1
            {
                Id = User.Item2.Id,
                UserName = IdentityUser.UserName,
                IdentityUserId = User.Item2.IdentityUserId,
                IsLocked = User.Item2.IsLocked,
                Email = User.Item2.Email,
                DOB = User.Item2.Dob,
                PhoneNumber = User.Item2.PhoneNumber,
                IsApproved = User.Item2.IsApproved,
                JobTitle = User.Item2.JobTitle,
                RoleId = UserRoles.Count() > 0 ? (await RoleManager.FindByNameAsync(UserRoles.FirstOrDefault())).Id : "",
                IsAdmin = User.Item2.IsAdmin
            };

            return Ok(UserResponse);
        }


        [HttpGet]
        [Route("CheckIfUserIsAdmin")]
        public async Task<IActionResult> CheckIfUserIsAdmin()
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var User = await this.IUserRepository.GetById(UserId);

            return Ok(User.Item2.IsAdmin);
        }

        [HttpGet]
        [Route("GetClientMenu")]
        public async Task<IActionResult> GetClientMenu()
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await IUserRepository.GetClientMenu(UserId);

            if (!Result.Item1)
                return Ok(BadRequest());

            return Ok(Result.Item2);
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string UserName, string NewPassword)
        {
            NewPassword = Helpers.DecodeBTOA(NewPassword);
            IdentityUser User = await UserManager.FindByNameAsync(UserName);
            if (User == null)
                return Ok(new ResponseMessage { Success = false, MessageEn = "Invalid User", MessageAr = "المستخدم غير موجود" });

            var PasswordValidator = new PasswordValidator<IdentityUser>();
            var ValidateNewPassword = await PasswordValidator.ValidateAsync(UserManager, User, NewPassword);
            if (ValidateNewPassword.ToString() != "Succeeded")
                return Ok(new ResponseMessage { Success = false, MessageEn = ValidateNewPassword.ToString() });

            await UserManager.RemovePasswordAsync(User);
            IdentityResult ChangePasswordResult = await UserManager.AddPasswordAsync(User, NewPassword);
            if (!ChangePasswordResult.Succeeded)
                return Ok(new ResponseMessage { Success = false, MessageEn = ChangePasswordResult.Errors.ToString() });

            return Ok(new ResponseMessage { Success = true, MessageEn = "Your password has been changed", MessageAr = "تم اعادة تعيين كلمة المرور" });
        }

        //[HttpPost]
        //[Route("ChangeLoggedInUserPassword")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ChangeLoggedInUserPassword(string OldPassword, string NewPassword)
        //{
        //    var TokenClaims = HttpContext.User;
        //    int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
        //    string UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value.ToString();

        //    string PlainOldPassword = Helpers.DecodeBTOA(OldPassword);
        //    string PlainNewPassword = Helpers.DecodeBTOA(NewPassword);
        //    IdentityUser User = await UserManager.FindByNameAsync(UserName);
        //    bool LoginCredentials = await UserManager.CheckPasswordAsync(User, PlainOldPassword);
        //    if (LoginCredentials)
        //    {
        //        var PasswordValidator = new PasswordValidator<IdentityUser>();
        //        var ValidateNewPassword = await PasswordValidator.ValidateAsync(UserManager, User, PlainNewPassword);
        //        if (ValidateNewPassword.ToString() != "Succeeded")
        //            return Ok(new ResponseMessage { Success = false, MessageEn = ValidateNewPassword.ToString() });

        //        await UserManager.RemovePasswordAsync(User);
        //        IdentityResult ChangePasswordResult = await UserManager.AddPasswordAsync(User, PlainNewPassword);
        //        if (!ChangePasswordResult.Succeeded)
        //            return Ok(new ResponseMessage { Success = false, MessageEn = ChangePasswordResult.Errors.ToString() });
        //    }
        //    else
        //    {
        //        return Ok(new ResponseMessage { Success = false, MessageEn = "Your old password is invalid", MessageAr = "كلمة المرور القديمة خاطئة" });
        //    }
        //    await ILogger.LogUserAction(new UserActionLog
        //    {
        //        Id = 0,
        //        UserId = UserId,
        //        Action = Convert.ToInt32(Operations.Update),
        //        Page = "User Profile",
        //        PageAr = "الملف الشخصي",
        //        Details = ("User Changed his/her password"),
        //        DetailsAr = ("قام المستخدم بتغيير كلمة المرور الخاصة به"),
        //        Date = DateTime.Now
        //    });
        //    return Ok(new ResponseMessage { Success = true, MessageEn = "Your password has been changed", MessageAr = "تم اعادة تعيين كلمة المرور" });
        //}

        [HttpPost]
        [Route("DeleteUsers")]
        public async Task<IActionResult> DeleteUsers(int Id)
        {

            var UserResult = await this.IUserRepository.DeleteUser(Id);
            if (!UserResult.Item1)
                return BadRequest();
            return Ok(UserResult);
        }

        [HttpGet]
        [Route("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IUserRepository.GetUserInfo(UserId);
            if (!Result.Item1)
                return BadRequest("Api encountered an error");

            return Ok(Result.Item2);
        }

        [HttpGet]
        [Route("GetUserFeatures")]
        public async Task<IActionResult> GetUserFeatures(string Lang)
        {
            //This line is used to extract claim data from token
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var Result = await this.IUserRepository.GetUserFeaturs(UserId, Lang);
            if (!Result.Item1)
                return BadRequest("Api encountered an error");

            return Ok(Result.Item2);
        }


    }
}
