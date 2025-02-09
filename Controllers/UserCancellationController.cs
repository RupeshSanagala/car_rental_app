using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/users/cancellations")]
    [ApiController]
    public class UserCancellationController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public UserCancellationController(CarRentalContext context)
        {
            _context = context;
        }

        // User requests a booking cancellation
        [HttpPost]
        public async Task<IActionResult> RequestCancellation([FromBody] CancellationRequestDto requestDto)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Booking_ID == requestDto.Booking_ID);

            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.EndDate < DateTime.UtcNow)
                return BadRequest("Cannot cancel a past booking.");

            var cancellation = CancellationConverter.CancellationRequestDtoToCancellation(requestDto);
            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cancellation request submitted successfully.", cancellationId = cancellation.CancellationId });
        }

        // User views their cancellation history
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserCancellations(int userId)
        {
            var cancellations = await _context.Cancellations
                .Where(c => c.Booking.User_ID == userId)
                .ToListAsync();

            return Ok(cancellations);
        }
    }
}

