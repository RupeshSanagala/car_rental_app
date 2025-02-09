using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using BCrypt.Net;
using Car_Rental_Backend_Application.Data.ResponseDtos;
using Car_Rental_Backend_Application.Data.RequestDtos;

namespace Car_Rental_Backend_Application.Data.Converters
{
    public static class UserConverters
    {
        public static UserResponseDto UserToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                
            };
        }

        public static User UserRequestDtoToUser(UserRegisterDto userDto)
        {
            return new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Address = userDto.Address,
                PhoneNumber = userDto.PhoneNumber,
                Role = "User",
                IsActive = true
            };
        }
    }
}
