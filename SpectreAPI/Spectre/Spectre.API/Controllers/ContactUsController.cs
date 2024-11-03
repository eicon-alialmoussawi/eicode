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
    [Route("api/ContactUs")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContactUsController : ControllerBase
    {
        private readonly ILogger ILogger;
        public readonly IContactUsRepository IContactUsRepository;
        private readonly IConfiguration Config;
        public ContactUsController(IConfiguration Config, ILogger ILogger, IContactUsRepository IContactUsRepository)
        {
            this.IContactUsRepository = IContactUsRepository;
            this.ILogger = ILogger;
            this.Config = Config;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await IContactUsRepository.GetAlll();
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item2  );
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {

            var Result = await IContactUsRepository.DeleteContact(Id);
            if (!Result.Item1)
                return Ok(BadRequest());
            return Ok(Result.Item1);
        }

        [HttpPost]
        [Route("SendReply")]
        public async Task<IActionResult> SendReply(ReplyTo_View reply)
        {

            string Authority = Config.GetValue<string>("SettingKeys:WebURL");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<div dir='ltr'>");
            sb.Append("<p>");
            sb.Append("Dear " + reply.Name + ",");
            sb.Append("</p>");
            sb.Append("<p>");
            sb.Append("Please find below our reply for your message: ");
            sb.Append("</p>");
            sb.Append("<p>");
            sb.Append("<strong>" + reply.Message + "</strong>");
            sb.Append("</p>");
            sb.Append("<p>");
            sb.Append(reply.Message);
            sb.Append("<p>");
            sb.Append("<p>");
            sb.Append(reply.Reply);
            sb.Append("<p>");

            sb.Append("<p>");
            sb.Append("Kind regards,");
            sb.Append("</p>");
            sb.Append("The Administrator");
            sb.Append("</br>");
            sb.Append("</div>");


            string To = reply.Email;
            string From = Config.GetValue<string>("SettingKeys:SendingEmail");
            string SMTPUserName = Config.GetValue<string>("SettingKeys:SmtpServerUsername");
            int Port = Config.GetValue<int>("SettingKeys:SmtpServerPort");
            string SMPTPassword = Config.GetValue<string>("SettingKeys:SmtpServerPassword");
            string HostServer = Config.GetValue<string>("SettingKeys:SmtpServer");
            if (!Helpers.SendEmail(To, sb.ToString(), From, "Account Registration", SMTPUserName, SMPTPassword, HostServer, Port))
                return Ok(false);
            return Ok(true);

        }
    }
}
