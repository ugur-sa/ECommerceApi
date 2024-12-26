namespace ECommerceApi.Services
{
    using Microsoft.AspNetCore.Identity;

    public class PasswordHasherService
    {
        private readonly PasswordHasher<string> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }

}
