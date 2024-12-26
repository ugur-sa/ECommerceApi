using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Application.Dtos
{
    public class LoginUserDto
    {
        [Required]
        public string Username { get; set; } = String.Empty;

        [Required]
        public string Password { get; set; } = String.Empty;
    }
}
