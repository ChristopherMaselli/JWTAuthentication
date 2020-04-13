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
using System.Net.Http;

namespace JWTAuthentication.Controllers
{
    [Route("api/Registration")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private IConfiguration _config;

        private readonly JWTAuthenticationContext _context;

        public  RegistrationController(IConfiguration config, JWTAuthenticationContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost]
        public async Task<string> PostUserAccountRegistration(UserAccount userAccount)
        {
            HttpResponse response = HttpContext.Response;
            Task<ActionResult<bool>> isEmailValid = EmailDataAuthenticator(userAccount);
            if (isEmailValid.Result.Value == false)
            {
                return "Email is Taken";
            }

            Task<ActionResult<bool>> isUsernameValid = UsernameDataAuthenticator(userAccount);
            if (isUsernameValid.Result.Value == false)
            {
                return "Username is Taken";
            }

            var pass = BCrypt.Net.BCrypt.HashPassword(userAccount.Password);

            UserAccount regiUser = new UserAccount();
            regiUser.UserName = userAccount.UserName;
            regiUser.Password = pass;
            regiUser.EmailAddress = userAccount.EmailAddress;

            _context.UserAccounts.Add(regiUser);

            await _context.SaveChangesAsync();
            //return CreatedAtAction("GetUserAccountRegistration", new { id = userAccountRegistration.UserId }, userAccountRegistration);

            return "Registration Complete! Welcome" + regiUser.UserName + "";
        }

        private async Task<ActionResult<bool>> UsernameDataAuthenticator(UserAccount registration)
        {
            UserAccount data = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == registration.UserName).FirstOrDefaultAsync<UserAccount>();
            if (data != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<ActionResult<bool>> EmailDataAuthenticator(UserAccount registration)
        {
            UserAccount data = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.EmailAddress == registration.EmailAddress).FirstOrDefaultAsync<UserAccount>();
            if (data != null)
            {
                return false;
            }
            else
            {
                return true;
            }
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