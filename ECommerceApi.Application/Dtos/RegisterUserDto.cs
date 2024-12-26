using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Application.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = String.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = String.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;

        public string? Role { get; set; }
    }
}