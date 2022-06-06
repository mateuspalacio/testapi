using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class TokenContext : DbContext
    {
        public TokenContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Token> Tokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Token>().HasKey(m => m.Id);
                base.OnModelCreating(builder);
            builder.Entity<Token>()
                .HasIndex(p => p.Value)
                .IsUnique(true);
        }
    }
}
