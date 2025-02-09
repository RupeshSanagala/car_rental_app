using System;

namespace Car_Rental_Backend_Application.Data.RequestDto_s
{
    public class BookingRequestDto
    {
        public int UserId { get; set; }
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
