using Api.Models;
using Microsoft.EntityFrameworkCore;
namespace Api.Data
{
    public class TesteDbContext : DbContext
    {
        public TesteDbContext(DbContextOptions<TesteDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
