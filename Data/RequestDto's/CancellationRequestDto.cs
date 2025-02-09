using System;
using System.ComponentModel.DataAnnotations;

namespace Car_Rental_Backend_Application.Data.RequestDto_s
{
    public class CancellationRequestDto
    {
        [Required]
        public int Booking_ID { get; set; }

        [Required]
        [StringLength(255)]
        public string Reason { get; set; }

        public DateTime CancellationDate { get; set; } = DateTime.UtcNow;
    }
}
