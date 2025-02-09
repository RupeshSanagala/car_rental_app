using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using Car_Rental_Backend_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public CancellationController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: api/cancellations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CancellationResponseDto>>> GetCancellations()
        {
            var cancellations = await _context.Cancellations.ToListAsync();
            var cancellationDtos = cancellations.Select(CancellationConverter.CancellationToCancellationResponseDto).ToList();
            return Ok(cancellationDtos);
        }

        // GET: api/cancellations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CancellationResponseDto>> GetCancellationById(int id)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation == null)
                return NotFound($"Cancellation with ID {id} not found.");

            return Ok(CancellationConverter.CancellationToCancellationResponseDto(cancellation));
        }

        // POST: api/cancellations
        [HttpPost]
        public async Task<ActionResult<CancellationResponseDto>> CreateCancellation(CancellationRequestDto cancellationDto)
        {
            if (cancellationDto == null)
                return BadRequest("Cancellation data is required.");

            var booking = await _context.Bookings.FindAsync(cancellationDto.Booking_ID);
            if (booking == null)
                return BadRequest("Invalid booking ID.");

            var cancellation = CancellationConverter.CancellationRequestDtoToCancellation(cancellationDto);
            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCancellationById), new { id = cancellation.CancellationId }, CancellationConverter.CancellationToCancellationResponseDto(cancellation));
        }

        // DELETE: api/cancellations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCancellation(int id)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation == null)
                return NotFound($"Cancellation with ID {id} not found.");

            _context.Cancellations.Remove(cancellation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
