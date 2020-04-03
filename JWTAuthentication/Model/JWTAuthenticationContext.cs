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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=GP2_DB;Username=postgres;Password=Squeakzilla1;");
    }
}
