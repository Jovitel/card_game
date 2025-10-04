using Microsoft.EntityFrameworkCore;

namespace CardGame.Server.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<CardGame.Shared.Models.User> Users { get; set; }
    }
}
