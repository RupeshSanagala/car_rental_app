using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Car_Rental_Backend_Application.Data;
using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.RequestDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Car_Rental_Backend_Application.Services
{
    public class AuthService
    {
        private readonly CarRentalContext _context;  // ✅ Only one declaration
        private readonly IConfiguration _config;  // ✅ Only one declaration

        public AuthService(CarRentalContext context, IConfiguration config)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));  // ✅ Fix null issue
            this._config = config ?? throw new ArgumentNullException(nameof(config));  // ✅ Fix null issue
        }

        // ✅ REGISTER METHOD
        public async Task<IActionResult> Register(UserRegisterDto registerDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDTO.Email))
            {
                return new BadRequestObjectResult("User already exists");
            }

            var user = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                Address = registerDTO.Address,
                PhoneNumber = registerDTO.PhoneNumber,
                Role = "User"
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, registerDTO.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new OkObjectResult("User registered successfully");
        }

        // ✅ LOGIN METHOD
        public async Task<IActionResult> Login(UserLoginDto loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null)
                return new UnauthorizedObjectResult("Invalid email or password");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);
            if (result == PasswordVerificationResult.Failed)
                return new UnauthorizedObjectResult("Invalid email or password");

            var token = GenerateJwtToken(user);
            var username = user.Username;
            return new OkObjectResult(new { username,token, role = user.Role });
        }

        // ✅ JWT TOKEN GENERATION
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config["Jwt:Key"]));  // ✅ Use `this._config`
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: this._config["Jwt:Issuer"],
                audience: this._config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
