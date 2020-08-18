using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

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

    public class UserData
    {
        [Key]
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public UserAccount UserAccount { get; set; }

        public string ImagePath { get; set; }

        public string MemberSince { get; set; }

        public string HoursPlayed { get; set; }

        public string Subscription { get; set; }
    }

    public class Game
    {
        [Key]
        public long GameId { get; set; }

        public string ImagePath { get; set; }

        public string Title { get; set; }

        public long OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public UserAccount UserAccount { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastPlayed { get; set; }

        public DateTime NextGameDateTime { get; set; }
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

    public class PurchaseItem //Make owner and name into a composite key sometime?
    {
        [Key]
        public long id { get; set; }

        public string ItemCode { get; set; }

        public long ItemOwnerId { get; set; }
        [ForeignKey("ItemOwnerId")]
        public UserAccount UserAccount { get; set; }

        public string PriceId { get; set; }

        public string ItemName { get; set; }

        public int ItemCost { get; set; }

        public string ItemDescription { get; set; }
    }

    public class StripeCustomer
    {
        [Key]
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public UserAccount UserAccount { get; set; }

        public string CustomerCode { get; set; }
    }

    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
