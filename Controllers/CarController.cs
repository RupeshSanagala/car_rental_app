using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public CarController(CarRentalContext context)
        {
            _context = context;
        }

        // ✅ Get all available cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarResponseDto>>> GetAvailableCars(Data.Entities.Car c)
        {
            var cars = await _context.Cars
                .Where(c => c.Avaliability_Status =="Avaliable")  // 🔹 Corrected field name
                .ToListAsync();

            var carDtos = cars.Select(CarConverters.CarToCarResponseDto).ToList();
            return Ok(carDtos);
        }

        // ✅ Get car details by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CarResponseDto>> GetCarById(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound($"Car with ID {id} not found.");

            return Ok(CarConverters.CarToCarResponseDto(car));
        }
    }
}
