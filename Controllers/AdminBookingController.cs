using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/admin/bookings")]
    [ApiController]
    public class AdminBookingController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public AdminBookingController(CarRentalContext context)
        {
            _context = context;
        }

        // ✅ Get all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetAllBookings()
        {
            var bookings = await _context.Bookings.Include(b => b.Car).ToListAsync();
            var bookingDtos = bookings.Select(BookingConverters.BookingToBookingResponseDto).ToList();
            return Ok(bookingDtos);
        }

        // ✅ Approve a booking
        [HttpPut("{bookingId}/approve")]
        public async Task<IActionResult> ApproveBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            booking.Status = "Approved";
            await _context.SaveChangesAsync();

            return Ok("Booking approved successfully.");
        }

        // ✅ Reject a booking
        [HttpPut("{bookingId}/reject")]
        public async Task<IActionResult> RejectBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            booking.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Ok("Booking rejected successfully.");
        }
    }
}
