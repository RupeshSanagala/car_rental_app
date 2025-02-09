using Car_Rental_Backend_Application.Data.Entities;
using Car_Rental_Backend_Application.Data.RequestDto_s;
using Car_Rental_Backend_Application.Data.ResponseDto_s;
using BCrypt.Net;

namespace Car_Rental_Backend_Application.Data.Converters
{
    public static class AdminConverters
    {
        public static AdminResponseDto AdminToAdminResponseDto(Admin admin)
        {
            return new AdminResponseDto
            {
                AdminId = admin.AdminId,
                Username = admin.Username,
                Email = admin.Email,
                IsActive = admin.IsActive
            };
        }

        public static Admin AdminRequestDtoToAdmin(AdminRequestDto adminDto)
        {
            return new Admin
            {
             
                Email = adminDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(adminDto.Password),
                IsActive = true
            };
        }
    }
}
