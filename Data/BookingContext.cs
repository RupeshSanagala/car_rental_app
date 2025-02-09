using Car_Rental_Backend_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_Backend_Application.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Cancellations)
                .WithOne(c => c.Booking)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
