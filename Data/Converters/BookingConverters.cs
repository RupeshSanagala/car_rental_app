using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using Car_Rental_Backend_Application.Models;

namespace Car_Rental_Backend_Application.Data.Converters
{
    public static class BookingConverters
    {
        public static BookingResponseDto BookingToBookingResponseDto(Booking booking)
        {
            return new BookingResponseDto
            {
                BookingId = booking.Booking_ID,
                UserId = booking.User_ID,
                CarId = booking.Car_ID,
                CarMake = booking.Car.Brand,
                CarModel = booking.Car.Model,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status
            };
        }
    }
}
