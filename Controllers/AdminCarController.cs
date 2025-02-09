using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/admin/cars")]
    [ApiController]
    public class AdminCarController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public AdminCarController(CarRentalContext context)
        {
            _context = context;
        }

        // ✅ Add a new car (Admin only)
        [HttpPost]
        public async Task<ActionResult<CarResponseDto>> AddCar([FromBody] CarRequestDto carDto)
        {
            if (carDto == null)
                return BadRequest("Car details are required.");

            var car = CarConverters.CarRequestDtoToCar(carDto);
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarById), new { id = car.Car_ID }, CarConverters.CarToCarResponseDto(car));
        }

        // ✅ Get all cars (Admin view)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarResponseDto>>> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();
            if (cars == null || !cars.Any())
                return NotFound("No cars found.");

            var carResponseDtos = cars.Select(car => CarConverters.CarToCarResponseDto(car)).ToList();
            return Ok(carResponseDtos);
        }

        // ✅ Get car by ID (Admin view)
        [HttpGet("{id}")]
        public async Task<ActionResult<CarResponseDto>> GetCarById(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound($"Car with ID {id} not found.");

            return Ok(CarConverters.CarToCarResponseDto(car));
        }

        // ✅ Update car details (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] CarRequestDto carDto)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound($"Car with ID {id} not found.");

            car.Brand = carDto.Brand;
            car.Model = carDto.Model;
           
            car.PricePerDay = (decimal)carDto.price;
            car.License_Plate = carDto.License_Plate;
            car.Avaliability_Status = carDto.Availability_Status;

            await _context.SaveChangesAsync();
            return Ok(CarConverters.CarToCarResponseDto(car));
        }

        // ✅ Delete a car (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound($"Car with ID {id} not found.");

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}