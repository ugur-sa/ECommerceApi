using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Dtos.User
{
    public class UpdateUserDto
    {
        public string? Username { get; set; } = String.Empty;
        public string? Email { get; set; } = String.Empty;
        public string? Role { get; set; } = String.Empty;
    }
}
