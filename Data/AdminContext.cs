using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_Backend_Application.Data
{
    public class AdminContext : DbContext
    {
        public AdminContext(DbContextOptions<AdminContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();
        }
    }
}
