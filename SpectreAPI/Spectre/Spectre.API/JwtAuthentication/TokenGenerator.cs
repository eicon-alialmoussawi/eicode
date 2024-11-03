 
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Spectre.API.JwtAuthentication
{
    public class TokenGenerator
    {
        private IConfiguration _config;
        public TokenGenerator(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJSONWebToken(UserTokenObj User)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtAuthentication:JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var Claims = new Claim[]
           {
               new Claim("IdentityUserId", User.IdentityUser),
                new Claim("UserId", User.UserId),
                new Claim("UserName", User.UserName),
                new Claim("Email", User.Email),
                new Claim("Lang", User.Lang),
                new Claim("IsAdmin", User.IsAdmin.ToString()),
           };

            var token = new JwtSecurityToken(_config["JwtAuthentication:JwtIssuer"],
              _config["JwtAuthentication:JwtIssuer"],
              Claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
