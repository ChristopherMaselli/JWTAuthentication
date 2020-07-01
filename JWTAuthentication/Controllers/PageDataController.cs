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
using Newtonsoft.Json;
namespace JWTAuthentication.Controllers
{
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

        [HttpGet("UserProfile")]
        public async Task<ActionResult<string>> UserProfileDetails(string token)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;

            //[FIX LATER! THERE IS PROBABLY A MORE EFFICIENT WAY OF DOING THIS THAN CALLING THE DATABASE TWICE!]
            UserAccount userAccount = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();
            UserData userData = await _context.UserDatas.Where<UserData>(UserData => UserData.UserId == userAccount.Id).FirstOrDefaultAsync<UserData>();
            UserAccData userAccData = new UserAccData(userAccount.UserName, userAccount.EmailAddress, userData.MemberSince, userData.HoursPlayed, userData.Subscription);

            var jsonTest = JsonConvert.SerializeObject(userAccData);
            return jsonTest;
        }

        [HttpGet("UserGames")]
        public async Task<ActionResult<string>> UserGameDetails(string token)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            UserAccount userAccount = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();
            
            Game[] games = await _context.Games.Where<Game>(Game => Game.OwnerId == userAccount.Id).ToArrayAsync<Game>();
  
            PlayerToGame[] players = await _context.PlayerToGames.Where<PlayerToGame>(PlayerToGame => PlayerToGame.UserId == userAccount.Id).ToArrayAsync<PlayerToGame>();

            List<UserData> playerList = new List<UserData>();

            foreach (PlayerToGame ptg in players)
            {
                UserData player = await _context.UserDatas.Where<UserData>(UserData => UserData.UserId == ptg.UserId).FirstOrDefaultAsync<UserData>();
                playerList.Add(player);
            }

            //use the username to get the id
            //use the id to get list of game id's
            //use the list of game id's to return a json list of the game details
            //Send back and de-serialize 
            //Make a looping list of things in front-end. 

            //[FIX LATER! THERE IS PROBABLY A MORE EFFICIENT WAY OF DOING THIS THAN CALLING THE DATABASE TWICE!]
            UserData userData = await _context.UserDatas.Where<UserData>(UserData => UserData.UserId == userAccount.Id).FirstOrDefaultAsync<UserData>();
            UserAccData userAccData = new UserAccData(userAccount.UserName, userAccount.EmailAddress, userData.MemberSince, userData.HoursPlayed, userData.Subscription);

            var jsonTest = JsonConvert.SerializeObject(userAccData);
            return jsonTest;
        }

        [Serializable]
        public class UserAccData
        {
            public string username;
            public string emailAddress;
            public string memberSince;
            public string hoursPlayed;
            public string subscription;

            public UserAccData(string userName, string emailAddress, string memberSince, string hoursPlayed, string subscription)
            {
                this.username = userName;
                this.emailAddress = emailAddress;
                this.memberSince = memberSince;
                this.hoursPlayed = hoursPlayed;
                this.subscription = subscription;
            }
        }
    }
}