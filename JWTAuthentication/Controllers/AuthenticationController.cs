using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTAuthentication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _config;

        private readonly JWTAuthenticationContext _context;

        public AuthenticationController(IConfiguration config, JWTAuthenticationContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> PostUserAccountLogin(UserAccount userAccount)
        {
            IActionResult response = Unauthorized();

            //If the data is Authenticated as Valid, make a token and return it
            if (UserAccountAuthenticator(userAccount.UserName, userAccount.Password).Result.Value == true)
            {
                var tokenStr = GenerateJSONWebToken(userAccount);
                response = Ok(new { token = tokenStr });
            }
            return response;
        }

        private async Task<ActionResult<bool>> UserAccountAuthenticator(string userName, string password)
        {
            UserAccount data = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();

            return BCrypt.Net.BCrypt.Verify(password, data.Password);
        }

        [HttpPost("Registration")]
        public async Task<string> PostUserAccountRegistration(UserAccount userAccount)
        {
            HttpResponse response = HttpContext.Response;

            //Check is Email is taken
            if (await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.EmailAddress == userAccount.EmailAddress).FirstOrDefaultAsync<UserAccount>() != null)
            {
                return "Email is Taken";
            }

            //Check if UserName is taken
            if (await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userAccount.UserName).FirstOrDefaultAsync<UserAccount>() == null)
            {
                return "Username is Taken";
            }

            //If not, make a proper Hash of the password
            var bPass = BCrypt.Net.BCrypt.HashPassword(userAccount.Password);

            UserAccount regiUser = new UserAccount();
            regiUser.UserName = userAccount.UserName;
            regiUser.Password = bPass;
            regiUser.EmailAddress = userAccount.EmailAddress;

            //Add the data to the Database and Save
            _context.UserAccounts.Add(regiUser);
            await _context.SaveChangesAsync();

            return "Registration Complete! Welcome" + regiUser.UserName + "";
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> AuthorizeToken(string token)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;

            if (UserAccountAuthenticator(userName, password).Result.Value == true)
            {
                return Ok(new { token = true });
            }
            return null;
        }
        private string GenerateJSONWebToken(UserAccount userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); //----------------------
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName), //-----------------------------
                new Claim(JwtRegisteredClaimNames.Prn, userInfo.Password),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

    }
}