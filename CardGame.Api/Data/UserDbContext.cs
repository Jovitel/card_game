using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CardGame.Shared.Models;

namespace CardGame.Server.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Friends)
                .HasConversion(
                    v => SerializeFriends(v),
                    v => DeserializeFriends(v)
                );
        }

        private static string SerializeFriends(List<int>? friends)
        {
            return JsonSerializer.Serialize(friends ?? new List<int>());
        }

        private static List<int> DeserializeFriends(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<int>();

            try
            {
                return JsonSerializer.Deserialize<List<int>>(json!) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }
    }
}
