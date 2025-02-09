using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using Car_Rental_Backend_Application.Models;

namespace Car_Rental_Backend_Application.Data.Converters
{
    public static class CancellationConverter
    {
        public static Cancellation CancellationRequestDtoToCancellation(CancellationRequestDto dto)
        {
            return new Cancellation
            {
                BookingId = dto.Booking_ID,
                Reason = dto.Reason,
                CancellationDate = dto.CancellationDate
            };
        }

        public static CancellationResponseDto CancellationToCancellationResponseDto(Cancellation cancellation)
        {
            return new CancellationResponseDto
            {
                Cancellation_ID = cancellation.CancellationId,
                Booking_ID = cancellation.BookingId,
                Reason = cancellation.Reason,
                CancellationDate = cancellation.CancellationDate
            };
        }
    }
}
