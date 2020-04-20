using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.Model
{
    public class JWTAuthenticationContext : DbContext
    {
        public JWTAuthenticationContext(DbContextOptions<JWTAuthenticationContext> options) : base(options)
        {

        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public DbSet<UserProfilePageData> UserProfilePageDatas { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<PlayerToGame> PlayerToGames { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=GP2_DB;Username=postgres;Password=Squeakzilla1;");
    }
}
