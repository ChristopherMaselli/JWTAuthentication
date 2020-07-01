using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthentication.Model
{
    public class UserAccount
    {
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string EmailAddress { get; set; }
    }
    public class Token
    {
        [Key]
        public string token { get; set; }
    }

    [Serializable]
    public class UserData
    {
        [Key]
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public UserAccount UserAccountId { get; set; }

        //[ForeignKey("user_account_id")]

        public string MemberSince { get; set; }

        public string HoursPlayed { get; set; }

        public string Subscription { get; set; }
    }

    public class Game
    {
        [Key]
        public long GameId { get; set; }

        public string ImageURL { get; set; }

        public string GameName { get; set; }
        public long OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public UserAccount UserAccount { get; set; }

        public List<UserData> players { get; set; }

        public int DateCreated { get; set; }

        public int NextGameDateTime { get; set; }
    }

    public class PlayerToGame
    {
        [Key]
        public long id { get; set; }

        public long GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public UserAccount UserAccount { get; set; }
    }
}
