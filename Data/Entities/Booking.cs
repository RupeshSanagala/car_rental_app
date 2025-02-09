using Car_Rental_Backend_Application.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_Backend_Application.Models
{
    public class Booking
    {
        [Key]
        public int Booking_ID { get; set; }

        [Required]
        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        [Required]
        public int Car_ID { get; set; }

        [ForeignKey("Car_ID")]
        public Car Car { get; set; } // Navigation Property to Car

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Booked"; // Status: Booked, Completed, Cancelled

        public ICollection<Cancellation> Cancellations { get; set; }
    }
}
