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

        public DbSet<UserData> UserDatas { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<PlayerToGame> PlayerToGames { get; set; }

        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        public DbSet<StripeCustomer> StripeCustomers { get; set; }

        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>()
                        .HasRequired(m => m.HomeTeam)
                        .WithMany(t => t.HomeMatches)
                        .HasForeignKey(m => m.HomeTeamId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserData>()
                        .HasRequired(m => m.GuestTeam)
                        .WithMany(t => t.AwayMatches)
                        .HasForeignKey(m => m.GuestTeamId)
                        .WillCascadeOnDelete(false);
        }
        */


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=db-postgresql-nyc1-82398-do-user-7936759-0.a.db.ondigitalocean.com;Port=25060;Database=defaultdb;Username=doadmin;Password=udmrmy1h75xoansx;SSLMode=Require;TrustServerCertificate=True;");
    }
}
