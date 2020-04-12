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
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        private readonly JWTAuthenticationContext _context;

        public LoginController(IConfiguration config, JWTAuthenticationContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAccountLogin(UserAccount userAccount)
        {
            //await _context.SaveChangesAsync();
            //return CreatedAtAction("GetUserAccountRegistration", new { id = userAccountRegistration.UserId }, userAccountRegistration);
            return Login(userAccount.UserName, userAccount.Password);
        }

        /*
        [HttpPost("/Token")]
        public IActionResult TokenAuth(string token)
        {
            return;
        }
        */

        public IActionResult Login(string username, string password)
        {
            //var pass = BCrypt.Net.BCrypt.HashPassword(password);
            UserAccount login = new UserAccount();
            login.UserName = username;
            login.Password = password;
            IActionResult response = Unauthorized();

            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenStr = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;
        }

        private UserAccount AuthenticateUser(UserAccount login)
        {
            UserAccount user = null;

            Task<ActionResult<bool>> isValid = DataAuthenticator(login);

            
            //using static User Info for test
            if (isValid.Result.Value == true)
            {
                user = new UserAccount { UserName = login.UserName, Password = login.Password };
            }
            return user;
        }   

        private async Task<ActionResult<bool>> DataAuthenticator(UserAccount login)
        {
            UserAccount data = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == login.UserName).FirstOrDefaultAsync<UserAccount>();

            return BCrypt.Net.BCrypt.Verify(login.Password, data.Password);

            /*

            UserAccount data = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == login.UserName && UserAccount.Password == login.Password).FirstOrDefaultAsync<UserAccount>();

            if (data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            */
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

        /*
        //[Authorize]
        [HttpPost]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;
            Console.WriteLine(claim[0].Value);
            Console.WriteLine(claim[1].Value);
            return "Welcome To: " + userName;
        }
        */

        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Value1", "Value2", "Value3" };
        }
    }
}