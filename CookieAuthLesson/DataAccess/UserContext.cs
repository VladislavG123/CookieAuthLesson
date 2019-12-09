using CookieAuthLesson.Models;
using Microsoft.EntityFrameworkCore;

namespace CookieAuthLesson.DataAccess
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Email = "admin@lol.com",
                    Password = "123456"
                });
        }
    }
}
