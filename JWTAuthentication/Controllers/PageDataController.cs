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

    /*
    [Holder Class Example]
    public class UserAccData
    {
        public UserAccount ua;
        public UserData ud;

        public userAccData(UserAccount uaa, UserData udd)
        {
            ua = uaa;
            ud = udd;
        }
    }
    */

    [Route("api/Data")]
    [ApiController]
    public class PageDataController : ControllerBase
    {
        private IConfiguration _config;

        private readonly JWTAuthenticationContext _context;

        public PageDataController(IConfiguration config, JWTAuthenticationContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("UserProfile")]
        public async Task<IActionResult<UserData>> UserProfileDetails(Token token)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token.token);
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;
            UserAccount accountData = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();
            UserData userData = await _context.UserDatas.Where<UserData>(UserData => UserData.UserId == accountData.UserId).FirstOrDefaultAsync<UserData>();

            /*UserAccData uad = new UserAccData(accountData, userData);*/



            return userData;


            


            return null;
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
        [HttpPost("Welcome")]
        public async Task<UserAccount> DecodeToken(Token token)
        {
            if (token == null)
            {
                return null;
            }
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token.token);
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;
            UserAccount user = new UserAccount();
            user.UserName = userName;
            user.Password = password;

            var verified = DataAuthenticator(user);
            if (verified.Result.Value == true)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
        */

        //[Authorize]
        [HttpPost("Token")]
        public async Task<IActionResult> AuthorizeToken(Token token)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token.token);
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;
            Console.WriteLine(claim[0].Value);
            Console.WriteLine(claim[1].Value);
            //Get the details of the user from the server:
            //Send into an authenticator to verify password against BCrypt
            //Get the details needed
            //Send back the UserAccount with the proper details. 
            return null;
        }


        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Value1", "Value2", "Value3" };
        }
    }
}