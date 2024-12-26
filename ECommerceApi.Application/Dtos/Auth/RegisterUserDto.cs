using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Application.Dtos.Auth
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Role { get; set; }
    }
}