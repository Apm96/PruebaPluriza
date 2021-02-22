using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProjectSales.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly WideWorldImportersContext context;

        public TokenController(IConfiguration config, WideWorldImportersContext wideWorldImportersContext)
        {
            configuration = config;
            context = wideWorldImportersContext; 
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Validate User Log in
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Token(User UserData)
        {
            if (UserData != null && UserData.Userr != null && UserData.Password != null)
            {
                var user = await GetUser(UserData.Userr, UserData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Name", user.Name),
                    new Claim("Userr", user.Userr),
                    new Claim("IdUserType", user.IdUserType.ToString()),
                    new Claim("Email", user.Email)
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Validate User Log in
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<User> GetUser(string user, string password)
        {
            return await context.User.FirstOrDefaultAsync(u => u.Userr == user && u.Password == password);
        }

    }
}