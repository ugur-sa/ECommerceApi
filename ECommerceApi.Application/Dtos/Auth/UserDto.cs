using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Dtos.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
    }
}
