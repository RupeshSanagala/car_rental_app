using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Rental_Backend_Application.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/admin/cancellations")]
    [ApiController]
    public class AdminCancellationController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public AdminCancellationController(CarRentalContext context)
        {
            _context = context;
        }

        // Admin views all cancellation requests
        [HttpGet]
        public async Task<IActionResult> GetAllCancellations()
        {
            var cancellations = await _context.Cancellations
                .Include(c => c.Booking)
                .ThenInclude(b => b.User)
                .ToListAsync();

            return Ok(cancellations);
        }

        // Admin approves or rejects a cancellation request
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCancellationStatus(int id, [FromBody] string status)
        {
            var cancellation = await _context.Cancellations.Include(c => c.Booking).ThenInclude(b => b.Car).FirstOrDefaultAsync(c => c.CancellationId == id);

            if (cancellation == null)
                return NotFound("Cancellation request not found.");

            if (status != "Approved" && status != "Rejected")
                return BadRequest("Invalid status. Allowed values: 'Approved', 'Rejected'.");

            cancellation.Status = status;
            await _context.SaveChangesAsync();

            // If approved, update car availability
            if (status == "Approved")
            {
                var car = cancellation.Booking.Car;
                if (car != null)
                {
                    car.Avaliability_Status = "Avaliable";
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { message = $"Cancellation request {status} successfully." });
        }
    }
}
