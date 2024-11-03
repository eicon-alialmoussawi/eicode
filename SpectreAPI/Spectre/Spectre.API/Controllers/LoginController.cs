using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spectre.API.JwtAuthentication;
using Spectre.API.Utilities;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly TokenGenerator TokenGenerator;
        private readonly IConfiguration Config;
        private readonly Helpers Helpers;
        private readonly IUserRepository IUserRepository;
        private readonly ILogger ILogger;
        private readonly IParamRepository IParamRepository;

        public LoginController(UserManager<IdentityUser> UserManager, ILogger ILogger, TokenGenerator TokenGenerator, IConfiguration Config, Helpers Helpers, IUserRepository IUserRepository, IParamRepository IParamRepository)
        {
            this.UserManager = UserManager;
            this.TokenGenerator = TokenGenerator;
            this.Config = Config;
            this.Helpers = Helpers;
            this.ILogger = ILogger;
            this.IUserRepository = IUserRepository;
            this.IParamRepository = IParamRepository;
        }
        [HttpPost]
        [Route("ValidateLogin")]
        public async Task<IActionResult> ValidateLogin([FromBody] LoginCredentials _LoginCredentials)
        {
            //var AllUsers = await this.IUserRepository.GetAllExistingUsers();


            string PlainPassword = Helpers.DecodeBTOA(_LoginCredentials.Password);
            IdentityUser User = await UserManager.FindByNameAsync(_LoginCredentials.UserName);
            if (User != null)
            {
                bool LoginCredentials = await UserManager.CheckPasswordAsync(User, PlainPassword);
                if (LoginCredentials)
                {
                    if (User.LockoutEnd > DateTimeOffset.Now)
                        return Ok(new ResponseMessage { Success = false, MessageEn = "User Is Locked", MessageAr = "المستخدم مقفل" });
                    else
                    {
                        User _User = (await IUserRepository.GetByIdentityId(User.Id)).Item2;
                        UserInfo_View userInfo = (await IUserRepository.GetUserInfo(_User.Id)).Item2;

                        if ((bool)_User.IsDeleted || userInfo.StartDate > DateTimeOffset.Now)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "Sorry you don't have access to the system", MessageAr = "آسف ليس لديك حق الوصول إلى النظام" });
                        if ((bool)!_User.IsApproved)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "User is not approved", MessageAr = "المستخدم غير مفعل" });
                        if ((bool)_User.IsLocked)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "User is Locked", MessageAr = "المستخدم مقفل" });

                        int BePositiveParamId = 2;
                        var _Parameter = (await IParamRepository.GetById(BePositiveParamId)).Item2;
                        int _AllowClientToChooseBePositive;

                        Int32.TryParse(_Parameter.Value, out _AllowClientToChooseBePositive);

                        //if (_Parameter != null && _Parameter.Value == "1")
                        //{
                        //    _AllowClientToChooseBePositive = true;
                        //}

                        string Token = TokenGenerator.GenerateJSONWebToken(new UserTokenObj { UserName = User.UserName, Email = User.Email, UserId = _User.Id.ToString(), IdentityUser = User.Id, Lang = _LoginCredentials.Lang, IsAdmin = (bool)_User.IsAdmin });

                        LoginResponse LoginResponse = new LoginResponse
                        {
                            UserId = _User.Id.ToString(),
                            Token = Token,
                            UserName = User.UserName,
                            Email = User.Email,
                            IsAdmin = _User.IsAdmin,
                            Lang = _LoginCredentials.Lang,
                            FirstName = _User.Name,
                            LastName = _User.LastName,
                            LoggedInBefore = _User.LoggedInBefore,
                            AllowClientToChooseBePositive = _AllowClientToChooseBePositive
                        };

                        string IPAddress = _LoginCredentials.IpAddress;

                        await ILogger.LogUserAction(new UserActionLog
                        {
                            Id = 0,
                            UserId = _User.Id,
                            Action = Convert.ToInt32(Operations.Add),
                            Page = "Login",
                            PageAr = "تسجيل الدخول",
                            Details = IPAddress,
                            DetailsAr = (""),
                            Date = DateTime.Now
                        });

                        if (_User.LoggedInBefore == null || _User.LoggedInBefore == false)
                            await IUserRepository.UpdateUserLogInStatus(_User.Id);

                        string Message = JsonConvert.SerializeObject(LoginResponse);
                        return Ok(new ResponseMessage { Success = true, MessageEn = Message });
                    }
                }
                else
                {
                    return Ok(new ResponseMessage { Success = false, MessageEn = "Wrong username or password", MessageAr = "اسم المستخدم او كلمة المرور غير صحيحة" });
                }
            }
            else
            {
                User _UserByEmail = (await IUserRepository.GetByEmail(_LoginCredentials.UserName)).Item2;
                if (_UserByEmail != null)
                {
                    UserInfo_View userInfo = (await IUserRepository.GetUserInfo(_UserByEmail.Id)).Item2;
                    IdentityUser AspUserByEmail = await UserManager.FindByNameAsync(_UserByEmail.UserName);

                    bool LoginCredentials = await UserManager.CheckPasswordAsync(AspUserByEmail, PlainPassword);
                    if (LoginCredentials)
                    {
                        if ((bool)_UserByEmail.IsDeleted || userInfo.StartDate > DateTimeOffset.Now)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "Sorry you don't have access to the system", MessageAr = "آسف ليس لديك حق الوصول إلى النظام" });
                        if ((bool)!_UserByEmail.IsApproved)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "User is not approved", MessageAr = "المستخدم غير مفعل" });
                        if ((bool)_UserByEmail.IsLocked)
                            return Ok(new ResponseMessage { Success = false, MessageEn = "User is Locked", MessageAr = "المستخدم مقفل" });

                        int BePositiveParamId = 2;
                        var _Parameter = (await IParamRepository.GetById(BePositiveParamId)).Item2;
                        int _AllowClientToChooseBePositive;

                        int.TryParse(_Parameter.Value, out _AllowClientToChooseBePositive);

                        //if (_Parameter != null && _Parameter.Value == "1")
                        //{
                        //    _AllowClientToChooseBePositive = true;
                        //}

                        string Token = TokenGenerator.GenerateJSONWebToken(new UserTokenObj { UserName = AspUserByEmail.UserName, Email = AspUserByEmail.Email, UserId = _UserByEmail.Id.ToString(), IdentityUser = AspUserByEmail.Id, Lang = _LoginCredentials.Lang, IsAdmin = (bool)_UserByEmail.IsAdmin });

                        LoginResponse LoginResponse = new LoginResponse
                        {
                            UserId = _UserByEmail.Id.ToString(),
                            Token = Token,
                            UserName = AspUserByEmail.UserName,
                            Email = AspUserByEmail.Email,
                            IsAdmin = _UserByEmail.IsAdmin,
                            Lang = _LoginCredentials.Lang,
                            FirstName = _UserByEmail.Name,
                            LastName = _UserByEmail.LastName,
                            LoggedInBefore = _UserByEmail.LoggedInBefore,
                            AllowClientToChooseBePositive = _AllowClientToChooseBePositive
                        };

                        string IPAddress = _LoginCredentials.IpAddress;

                        await ILogger.LogUserAction(new UserActionLog
                        {
                            Id = 0,
                            UserId = _UserByEmail.Id,
                            Action = Convert.ToInt32(Operations.Add),
                            Page = "Login",
                            PageAr = "تسجيل الدخول",
                            Details = IPAddress,
                            DetailsAr = (""),
                            Date = DateTime.Now
                        });

                        if (_UserByEmail.LoggedInBefore == null || _UserByEmail.LoggedInBefore == false)
                            await IUserRepository.UpdateUserLogInStatus(_UserByEmail.Id);


                        string Message = JsonConvert.SerializeObject(LoginResponse);
                        return Ok(new ResponseMessage { Success = true, MessageEn = Message });
                    }
                    else
                        return Ok(new ResponseMessage { Success = false, MessageEn = "Wrong username or password", MessageAr = "اسم المستخدم او كلمة المرور غير صحيحة" });

                }
                else
                    return Ok(new ResponseMessage { Success = false, MessageEn = "Invalid user, please register for a new account.", MessageAr = "المستخدم غير موجود، يرجى التسجيل للحصول على حساب جديد." });
            }
        }

        [HttpGet]
        [Route("RenewUserToken")]
        public async Task<IActionResult> RenewUserToken(string UserName, string Lang)
        {
            IdentityUser User = await UserManager.FindByNameAsync(UserName);
            if (User != null)
            {

                if (User.LockoutEnd > DateTimeOffset.Now)
                    return Ok(new ResponseMessage { Success = false, MessageEn = "User Is Locked", MessageAr = "المستخدم مقفل" });
                else
                {
                    User _User = (await IUserRepository.GetByIdentityId(User.Id)).Item2;

                    if ((bool)!_User.IsApproved)
                        return Ok(new ResponseMessage { Success = false, MessageEn = "User is not approved", MessageAr = "المستخدم غير مفعل" });
                    if ((bool)_User.IsLocked)
                        return Ok(new ResponseMessage { Success = false, MessageEn = "User is Locked", MessageAr = "المستخدم مقفل" });

                    string Token = TokenGenerator.GenerateJSONWebToken(new UserTokenObj { UserName = User.UserName, Email = User.Email, UserId = _User.Id.ToString(), IdentityUser = User.Id, Lang = Lang, IsAdmin = (bool)_User.IsAdmin });

                    LoginResponse LoginResponse = new LoginResponse
                    {
                        UserId = _User.Id.ToString(),
                        Token = Token,
                        UserName = User.UserName,
                        Email = User.Email,
                        IsAdmin = _User.IsAdmin,
                        Lang = Lang,
                        FirstName = _User.Name,
                        LastName = _User.LastName
                    };

                    string Message = JsonConvert.SerializeObject(LoginResponse);
                    return Ok(Message);
                }

            }
            else
            {
                return Ok(new ResponseMessage { Success = false, MessageEn = "Wrong username or password", MessageAr = "اسم المستخدم او كلمة المرور غير صحيحة" });
            }


        }

        [HttpGet]
        [Route("GetAllExistingUsers")]
        public async Task<IActionResult> GetAllExistingUsers()
        {

            var AllUsers = await this.IUserRepository.GetAllExistingUsers();
            if (!AllUsers.Item1)
                return BadRequest("Api encountered an error");

            return Ok(AllUsers.Item2);
        }
        [HttpGet]
        [Route("CheckTokeValidity")]
        public async Task<IActionResult> CheckTokeValidity(string Token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(Token);

            if (jwtSecurityToken.ValidTo < DateTime.UtcNow.AddSeconds(10))
                return Ok("UnValid");


            return Ok("Valid");
        }
        [HttpGet]
        [Route("GetAllPerm")]
        public async Task<IActionResult> GetAllPerm()
        {


            //PermutationCalculator s = new PermutationCalculator();
            //var ans =  s.Permute(new string[] { "200", "300", "900" });

            //string[] SelectedBands = { "200", "300", "2100" };
            return Ok("Totos");
        }

        [HttpGet]
        [Route("GetIPAddress")]
        public async Task<IActionResult> GetIPAddress()
        {
            string data = "";
            string url = "https://api.ipify.org";
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            data = await response.Content.ReadAsStringAsync();

            return Ok(data);

        }

        [HttpGet]
        [Route("GetLoginDetails")]
        public async Task<IActionResult> GetLoginDetails(DateTime date, int UserId)
        {

            var Result = await this.ILogger.GetLoginDetails(date, UserId);
            return Ok(Result);
        }
    }
}
