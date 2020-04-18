using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.Model
{
    public class UserAccount
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string EmailAddress { get; set; }
    }
    public class Token
    {
        [Key]
        public string token { get; set; }
    }

    public class UserData
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set;}
        public string MemberSince { get; set; }

        public string HoursPlayed { get; set; }

        public string Subscription { get; set; }
    }

    public class Game
    {
        [Key]
        public string GameId { get; set; }

        public string GameName { get; set; }

        public string Owner { get; set; }

        public int DateCreated { get; set; }

        public int NextGameDateTime { get; set; }
    }

    public class PlayerToGame
    {
        [Key]
        public int id { get; set; }
        public string GameId { get; set; }

        public string UserId { get; set; }
    }
}
