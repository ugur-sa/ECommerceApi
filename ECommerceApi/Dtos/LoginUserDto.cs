using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Dtos
{
    public class LoginUserDto
    {
        [Required]
        public string Username { get; set; } = String.Empty;

        [Required]
        public string Password { get; set; } = String.Empty;
    }
}
