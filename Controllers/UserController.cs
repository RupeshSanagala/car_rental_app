using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.RequestDtos;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using Car_Rental_Backend_Application.Data.ResponseDtos;
using Car_Rental_Backend_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly IUserService _userService;
        public UsersController(CarRentalContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

       

      

        // ✅ User registration
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto userDto)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
                return BadRequest("User with this email already exists.");

            var user = UserConverters.UserRequestDtoToUser(userDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, UserConverters.UserToUserResponseDto(user));
        }

        // ✅ User login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return Unauthorized("Invalid credentials.");

            return Ok(new { message = "Login successful", user });
        }

        // ✅ View user details
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(UserConverters.UserToUserResponseDto(user));
        }

        // ✅ Update user details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserRegisterDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Address = userDto.Address; 

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete user (only if no active bookings)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.Include(u => u.Bookings).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound("User not found.");
            if (user.Bookings.Any()) return BadRequest("Cannot delete user with active bookings.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize] // Ensures only authenticated users can access
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized("User email not found in token");

            var userProfile = await _userService.GetUserProfileByEmailAsync(email);

            if (userProfile == null)
                return NotFound("User profile not found");

            return Ok(userProfile);
        }


        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto userProfileDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("User ID not found");

            // 🔹 Convert userId to int
            if (!int.TryParse(userIdString, out int userId))
                return BadRequest("Invalid user ID format");

            var result = await _userService.UpdateUserProfileAsync(userId, userProfileDto);
            if (!result)
                return BadRequest("Failed to update profile");

            return Ok(new { message = "Profile updated successfully" });
        }


    }
}
