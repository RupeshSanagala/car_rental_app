using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_Backend_Application.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.User_ID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
