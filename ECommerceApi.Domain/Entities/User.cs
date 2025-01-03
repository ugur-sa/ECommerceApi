using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Address {  get; set; } = String.Empty;
        public string Role { get; set; } = "User";
    }
}

