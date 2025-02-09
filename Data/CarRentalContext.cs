using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_Backend_Application.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Car>()
                .HasMany(c => c.Bookings)
                .WithOne(b => b.Car)
                .HasForeignKey(b => b.Car_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.User_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Cancellations)
                .WithOne(c => c.Booking)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        
    }
}
