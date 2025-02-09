using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_Backend_Application.Models
{
    public class Cancellation
    {
        [Key]
        public int CancellationId { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }  // Navigation property

        [Required]
        public DateTime CancellationDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        public ICollection<Cancellation> Cancellations { get; set; }
    }
}
