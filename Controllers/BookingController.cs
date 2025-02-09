using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Car_Rental_Backend_Application.Models;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public BookingController(CarRentalContext context)
        {
            _context = context;
        }

        // ✅ GET: Get all bookings (Admin)
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .ToListAsync();

            var bookingDtos = bookings.Select(BookingConverters.BookingToBookingResponseDto).ToList();
            return Ok(bookingDtos);
        }

        // ✅ GET: Get bookings for a specific user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUserBookings(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.User_ID == userId)
                .ToListAsync();

            if (!bookings.Any())
                return NotFound("No bookings found for this user.");

            var bookingDtos = bookings.Select(BookingConverters.BookingToBookingResponseDto).ToList();
            return Ok(bookingDtos);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingRequestDto bookingDto)
        {
            var car = await _context.Cars.FindAsync(bookingDto.CarId);
            if (car == null)
                return BadRequest("Car not found.");

            if (car.Avaliability_Status =="Booked")
                return BadRequest("Car is currently unavailable.");

            // Check if the car is already booked within the requested dates
            bool isCarBooked = await _context.Bookings.AnyAsync(b =>
                b.Car_ID == bookingDto.CarId &&
                b.Status != "Canceled" &&
                ((b.StartDate <= bookingDto.EndDate && b.EndDate >= bookingDto.StartDate))
            );

            if (isCarBooked)
                return BadRequest("Car is already booked for these dates.");

            if (bookingDto.StartDate >= bookingDto.EndDate)
                return BadRequest("Invalid booking dates.");

            int totalDays = (bookingDto.EndDate - bookingDto.StartDate).Days;
            decimal totalPrice = totalDays * car.PricePerDay;

            var booking = new Booking
            {
                User_ID = bookingDto.UserId,
                Car_ID = bookingDto.CarId,
                StartDate = bookingDto.StartDate,
                EndDate = bookingDto.EndDate,
                TotalPrice = totalPrice,
                Status = "Pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserBookings), new { userId = booking.User_ID }, BookingConverters.BookingToBookingResponseDto(booking));
        }


        // ✅ PUT: Cancel a booking (User/Admin)
        [HttpPut("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.Status != "Pending")
                return BadRequest("Only pending bookings can be canceled.");

            booking.Status = "Canceled";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking canceled successfully." });
        }

        // ✅ PUT: Approve a booking (Admin)
        [HttpPut("{bookingId}/approve")]
        public async Task<IActionResult> ApproveBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.Status != "Pending")
                return BadRequest("Only pending bookings can be approved.");

            booking.Status = "Approved";

            var car = await _context.Cars.FindAsync(booking.Car_ID);
            if (car != null)
                car.Avaliability_Status = "Booked"; // Mark car as unavailable once booked

            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking approved successfully." });
        }
    }
}
