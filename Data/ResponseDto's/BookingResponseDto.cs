using System;

namespace Car_Rental_Backend_Application.Data.ResponseDto_s
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}
