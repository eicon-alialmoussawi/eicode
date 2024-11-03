using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [Route("api/CompanyPackages")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyPackageController : ControllerBase
    {

        private readonly IMapper mapper;
        private readonly ICompanyPackageRepository ICompanyPackageRepository;
        private readonly IPackageRepository IPackageRepository;
        private readonly IUserRepository IUserRepository;
        private readonly ILogger ILogger;
        private readonly UserManager<IdentityUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly IConfiguration Config;

        public CompanyPackageController(IPackageRepository IPackageRepository, IConfiguration Config, RoleManager<IdentityRole> RoleManager, UserManager<IdentityUser> UserManager, IUserRepository IUserRepository, IMapper mapper, ILogger ILogger, ICompanyPackageRepository ICompanyPackageRepository)
        {
            this.IUserRepository = IUserRepository;
            this.ICompanyPackageRepository = ICompanyPackageRepository;
            this.mapper = mapper;
            this.ILogger = ILogger;
            this.UserManager = UserManager;
            this.RoleManager = RoleManager;
            this.IPackageRepository = IPackageRepository;
            this.Config = Config;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await ICompanyPackageRepository.GetAllWithDetails();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);
        }


        [HttpPost]
        [Route("SaveCompanyPackage")]
        public async Task<IActionResult> SaveCompanyPackage(CompanyPackage_View companyView)
        {

            List<CompanyUsers_Result> usersResult = new List<CompanyUsers_Result>();
            List<UnacceptedUsers_View> users = new List<UnacceptedUsers_View>();

            var valid = true;
            var UnaccpetedUserName = "";
            foreach (User_View user in companyView.users)
            {
                IdentityUser ExistUserName = await UserManager.FindByNameAsync(user.UserName);
                UnacceptedUsers_View view = new UnacceptedUsers_View();
                if (ExistUserName != null)
                {
                    valid = false;
                    view.Type = "Username";
                    view.Value = user.UserName;
                    view.Message = " already exists in the system.";
                    users.Add(view);
                }
                   
                IdentityUser ExistEmail = await UserManager.FindByEmailAsync(user.Email);
                if (ExistEmail != null)
                {
                    valid = false;
                    view.Type = "Email";
                    view.Value = user.Email;
                    view.Message = " already exists in the system.";
                    users.Add(view);
                }
            }

            if (!valid)
            {

                return Ok(new ResponeMessage_2 { Success = false, users = users });
            }

            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            companyView.CreationDate = DateTime.Now;

            var Result = await this.ICompanyPackageRepository.Save(companyView);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
            {
                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = Convert.ToInt32(Operations.Add),
                    Page = "Company Packages",
                    PageAr = "حزم الشرك",
                    Details = ("Company Package for  Compant (" + companyView.company.Name + ") has been added"),
                    DetailsAr = ("تمت إضافة حزمة الشركة لشركة " + companyView.company.Name + " "),
                    Date = DateTime.Now
                });

                foreach(User_View user in companyView.users)
                {

                    IdentityUser _User = new IdentityUser
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = "",
                    };

                    IdentityResult IdentityResult = await UserManager.CreateAsync(_User, user.Password);

                    User User = new User();
                    User.Id = 0;
                    User.Name = user.Name;
                    User.LastName = "";


                    User.UserName = user.UserName;
                    User.Email = user.Email;
                    User.PhoneNumber = "";
                    User.IdentityUserId = _User.Id;
                    User.CreationDate = DateTime.Now;
                    User.IsDeleted = false;
                    User.IsApproved = true;
                    User.IsAdmin = false;
                    User.IsLocked = !companyView.IsActive;
                    User.UserType = "U_CLI";
                    User.CompanyId = Result.Item2;

                    var UserResult = await this.IUserRepository.Create(User);
                    CompanyUsers_Result _Result = new CompanyUsers_Result();
                    _Result.Name = user.Name;
                    _Result.UserName = user.UserName;
                    _Result.Password = user.Password;

                    usersResult.Add(_Result);

                    var Role = await RoleManager.FindByIdAsync("82736e4c-d61e-47b4-ac40-a8baabbcdd4c");
                    await UserManager.AddToRoleAsync(_User, Role.Name);

                }

                string Authority = Config.GetValue<string>("SettingKeys:WebURL");

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("<div dir='ltr' class='boxed'>");
                sb.Append("<p>");
                sb.Append("Dear " + companyView.company.Name + ",");
                sb.Append("</p>");
                sb.Append("<p>");
                sb.Append("Please note that your account in Spectre has been created: ");
                sb.Append("</p>");

                sb.Append("The following accounts have been created: ");
                sb.Append("<ol>");
                foreach (CompanyUsers_Result elm in usersResult)
                {
                    sb.Append("<li>");
                    sb.Append("<strong>Name: </strong> " + elm.Name);
                    sb.Append("<ul>");
                    sb.Append("<li>");
                    sb.Append("<strong>Username: </strong> " + elm.UserName);
                    sb.Append("</li>");
                    sb.Append("<li>");
                    sb.Append("<strong>Password: </strong> " + elm.Password);
                    sb.Append("</li>");
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                sb.Append("</ol>");

                sb.Append("</br>");
                sb.Append("<p>");
                sb.Append("Kind regards,");
                sb.Append("</p>");
                sb.Append("The Administrator");
                sb.Append("</br>");
                sb.Append("</div>");
                sb.Append("<hr/>");

                sb.Append("<div dir='rtl' class='boxed'>");
                sb.Append("<p dir='rtl'>");
                sb.Append("عزيزي " + companyView.company.Name + ",");
                sb.Append("</p>");
                sb.Append("<p dir='rtl'>");
                sb.Append("يرجى ملاحظة أنه تم إنشاء حسابك في Specter:");
                sb.Append("</p>");
                sb.Append("<p dir='rtl'>");
                sb.Append("تم إنشاء الحسابات التالية");
                sb.Append("</p>");
                sb.Append("<ol dir='rtl'>");
                foreach (CompanyUsers_Result elm in usersResult)
                {
                    sb.Append("<li dir='rtl'>");
                    sb.Append("<strong>الاسم: </strong> " + elm.Name);
                    sb.Append("<ul dir='rtl'>");
                    sb.Append("<li dir='rtl'>");
                    sb.Append("<strong>اسم المستخدم: </strong> " + elm.UserName);
                    sb.Append("</li>");
                    sb.Append("<li dir='rtl'>");
                    sb.Append("<strong>كلمة المرور: </strong> " + elm.Password);
                    sb.Append("</li>");
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                sb.Append("</ol>");

                sb.Append("</br>");
                sb.Append("<p dir='rtl'>");
                sb.Append("أطيب التحيات,");
                sb.Append("</p>");
                sb.Append("المدير");
                sb.Append("</br>");
                sb.Append("</div>");
                sb.Append("<hr/>");

                //In French 
                sb.Append("<div dir='ltr' class='boxed'>");
                sb.Append("<p>");
                sb.Append("cher " + companyView.company.Name + ",");
                sb.Append("</p>");
                sb.Append("<p>");
                sb.Append("Votre compte en spectre a ete cree: ");
                sb.Append("</p>");

                sb.Append("Le compte suivant a ete cree: ");
                sb.Append("<ol>");
                foreach (CompanyUsers_Result elm in usersResult)
                {
                    sb.Append("<li>");
                    sb.Append("<strong>Nom: </strong> " + elm.Name);
                    sb.Append("<ul>");
                    sb.Append("<li>");
                    sb.Append("<strong>Nom de passe: </strong> " + elm.UserName);
                    sb.Append("</li>");
                    sb.Append("<li>");
                    sb.Append("<strong>Mot de passe: </strong> " + elm.Password);
                    sb.Append("</li>");
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                sb.Append("</ol>");

                sb.Append("</br>");
                sb.Append("<p>");
                sb.Append("Sinceres salutation,");
                sb.Append("</p>");
                sb.Append("Le Administrateur");
                sb.Append("</br>");
                sb.Append("</div>");
                
 
                

                string To = companyView.company.Email;
                string From = Config.GetValue<string>("SettingKeys:SendingEmail");
                string SMTPUserName = Config.GetValue<string>("SettingKeys:SmtpServerUsername");
                int Port = Config.GetValue<int>("SettingKeys:SmtpServerPort");
                string SMPTPassword = Config.GetValue<string>("SettingKeys:SmtpServerPassword");
                string HostServer = Config.GetValue<string>("SettingKeys:SmtpServer");
                Helpers.SendEmail(To, sb.ToString(), From, "Account Registration", SMTPUserName, SMPTPassword, HostServer, Port);
                                    
                return Ok(new ResponeMessage_2 { Success = true, users = users });
            }
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            CompanyPackageDetails view = new CompanyPackageDetails();
            var _result = await ICompanyPackageRepository.GetWithId(Id);
            var _company = await ICompanyPackageRepository.GetCompanyById(_result.CompanyId);
            var _users = await IUserRepository.GetUsersByCompanyId(_result.CompanyId);
            var _packagePermissions = await IPackageRepository.GetPackagePagePermissionDetails(_result.PackageId);
            var _companyPackageDetails = await ICompanyPackageRepository.GetDetails(Id);


            view.Id = _result.Id;
            view.PackageId = _result.PackageId;
            view.CreationDate = _result.CreationDate;
            view.StartDate = _result.StartDate;
            view.EndDate = _result.EndDate;
            view.Price = _result.Price;
            view.Currency = _result.Currency;
            view.NumberOfUsers = _result.NumberOfUsers;
            view.IsActive = _result.IsActive;
            view.IsDeleted = _result.IsDeleted;
            view.Company = _company.Item2;
            view.Users = _users.Item2.ToList();
            view.PackagePermissions = _packagePermissions.Item2.ToList();
            view.CPackageDetails = _companyPackageDetails.Item2.ToList();

            return Ok(view);
        }


        [HttpPost]
        [Route("UpdateCompanyPackage")]
        public async Task<IActionResult> UpdateCompanyPackage(CompanyPackageDetails companyView)
        {

            List<CompanyUsers_Result> usersResult = new List<CompanyUsers_Result>();
            List<UnacceptedUsers_View> users = new List<UnacceptedUsers_View>();

            var valid = true;
            var UnaccpetedUserName = "";
            List<int> UserIds = new List<int>();

            foreach (User_View2 user in companyView.Users)
            {
                UserIds.Add(user.Id);
                if (user.Id <= 0)
                {
                    IdentityUser ExistUserName = await UserManager.FindByNameAsync(user.UserName);
                    UnacceptedUsers_View view = new UnacceptedUsers_View();
                    if (ExistUserName != null)
                    {
                        valid = false;
                        view.Type = "Username";
                        view.Value = user.UserName;
                        view.Message = " already exists in the system.";
                        users.Add(view);
                    }

                    IdentityUser ExistEmail = await UserManager.FindByEmailAsync(user.Email);
                    if (ExistEmail != null)
                    {
                        valid = false;
                        view.Type = "Email";
                        view.Value = user.Email;
                        view.Message = " already exists in the system.";
                        users.Add(view);
                    }
                }
            }

            if (!valid)
            {

                return Ok(new ResponeMessage_2 { Success = false, users = users });
            }

            string Ids = string.Join(",", UserIds); 


            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            companyView.CreationDate = DateTime.Now;

            var Result = await this.ICompanyPackageRepository.Update(companyView);
            if (!Result.Item1)
                return Ok(BadRequest());
            else
            {
                await ILogger.LogUserAction(new UserActionLog
                {
                    Id = 0,
                    UserId = UserId,
                    Action = Convert.ToInt32(Operations.Update),
                    Page = "Company Packages",
                    PageAr = "حزم الشرك",
                    Details = ("Company Package for  Company (" + companyView.Company.Name + ") has been updated"),
                    DetailsAr = ("تمت تععيل حزمة الشركة لشركة " + companyView.Company.Name + " "),
                    Date = DateTime.Now
                });

                var deletedUsers = IUserRepository.DeleteCompanyUsers(Ids, companyView.Company.Id, !companyView.IsActive);

                foreach (User_View2 user in companyView.Users)
                {
                    if (user.IsDeleted == null || user.IsDeleted == false)
                    {
                        if (user.Id <= 0)
                        {
                            IdentityUser _User = new IdentityUser
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                PhoneNumber = "",
                            };

                            IdentityResult IdentityResult = await UserManager.CreateAsync(_User, user.Password);

                            User User = new User();
                            User.Id = 0;
                            User.Name = user.Name;
                            User.LastName = "";


                            User.UserName = user.UserName;
                            User.Email = user.Email;
                            User.PhoneNumber = "";
                            User.IdentityUserId = _User.Id;
                            User.CreationDate = DateTime.Now;
                            User.IsDeleted = false;
                            User.IsApproved = true;
                            User.IsAdmin = false;
                            User.IsLocked = !companyView.IsActive;
                            User.UserType = "U_CLI";
                            User.CompanyId = Result.Item2;

                            var UserResult = await this.IUserRepository.Create(User);
                            CompanyUsers_Result _Result = new CompanyUsers_Result();
                            _Result.Name = user.Name;
                            _Result.UserName = user.UserName;
                            _Result.Password = user.Password;

                            usersResult.Add(_Result);

                            var Role = await RoleManager.FindByIdAsync("82736e4c-d61e-47b4-ac40-a8baabbcdd4c");
                            await UserManager.AddToRoleAsync(_User, Role.Name);
                        }
                        
                    }
                }
                return Ok(new ResponeMessage_2 { Success = true, users = users });
            }
        }



        [HttpGet]
        [Route("GetExpiredCompanyPackages")]
        public async Task<IActionResult> GetExpiredCompanyPackages()
        {

            var Result = await ICompanyPackageRepository.GetExpiredCompanyPackages();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2);
        }
    }
}
