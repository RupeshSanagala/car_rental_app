using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Converters;
using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.RequestDtos;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using Car_Rental_Backend_Application.Data.ResponseDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Rental_Backend_Application.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public AdminController(CarRentalContext context)
        {
            _context = context;
        }

        // ✅ View all users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users.Select(UserConverters.UserToUserResponseDto));
        }

        [HttpPost("login")]
        public async Task<IActionResult> AdminLogin([FromBody] AdminRequestDto loginDto)
        {
            var admin = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Role == "Admin");

            if (admin == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { message = "Login successful", adminId = admin.UserId, adminName = admin.Username });
        }

        // ✅ View user details
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(UserConverters.UserToUserResponseDto(user));
        }

        // ✅ Add new user
        [HttpPost("users")]
        public async Task<ActionResult<UserResponseDto>> AddUser(UserRegisterDto userDto)
        {
            var user = UserConverters.UserRequestDtoToUser(userDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, UserConverters.UserToUserResponseDto(user));
        }

        // ✅ Update user details
        [HttpPut("users/{id}")]
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

        // ✅ Delete user (if no active bookings)
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.Include(u => u.Bookings).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound("User not found.");
            if (user.Bookings.Any()) return BadRequest("Cannot delete user with active bookings.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Reset user password
        [HttpPut("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok("Password reset successfully.");
        }

        // ✅ Activate/deactivate user account
        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.IsActive = !user.IsActive;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok($"User status updated to {(user.IsActive ? "Active" : "Inactive")}");
        }

        // ✅ View user booking history
        [HttpGet("users/{id}/bookings")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUserBookings(int id)
        {
            var user = await _context.Users.Include(u => u.Bookings).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound("User not found.");
            return Ok(user.Bookings.Select(BookingConverters.BookingToBookingResponseDto));
        }

        // ✅ Assign roles (User/Admin)
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            if (role != "User" && role != "Admin") return BadRequest("Invalid role.");
            user.Role = role;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok("User role updated successfully.");
        }
    }
}
