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

        [HttpPost("UserGames")]
        public async Task<IActionResult> UserGameDetails(Token tokenVar)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenVar.token);
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            UserAccount userAccount = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();
            
            Game[] games = await _context.Games.Where(e => e.OwnerId == userAccount.Id).ToArrayAsync();

            //player to game is just the list of player ID's, use that to get the user-datas, then put that list of userdatas into the game array

            GamePackage[] gamePackageList = new GamePackage[games.Length];

            for (int i = 0; i<games.Length; i++)
            {
                List<UserData> userDataList = new List<UserData>();
                gamePackageList[i] = new GamePackage(games[i], userDataList);
                //Get the player Id's
                PlayerToGame[] playerIds = await _context.PlayerToGames.Where<PlayerToGame>(PlayerToGame => PlayerToGame.GameId == games[i].GameId).ToArrayAsync<PlayerToGame>();
                for (int j = 0; j<playerIds.Length; j++)
                {
                    //Get the userDatas from those ID's 
                    UserData userData = await _context.UserDatas.Where<UserData>(UserData => UserData.UserId == playerIds[j].UserId).FirstOrDefaultAsync<UserData>();
                    gamePackageList[i].playerList.Add(userData);
                }
                
            }

            var jsonGameList = JsonConvert.SerializeObject(gamePackageList);

            IActionResult response = Ok(new { gameList = jsonGameList });
            
            return response;
            //use the username to get the id
            //use the id to get list of game id's
            //use the list of game id's to return a json list of the game details
            //Send back and de-serialize 
            //Make a looping list of things in front-end.
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

        public class GamePackage
        {
            public Game game;
            public List<UserData> playerList;

            public GamePackage(Game game, List<UserData> playerList)
            {
                this.game = game;
                this.playerList = playerList;
            }
        }



    }
}