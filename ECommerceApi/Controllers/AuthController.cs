﻿using Azure.Core;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceApi.Application.Dtos.Auth;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Dtos.Category;
using ECommerceApi.Infrastructure.Repositories;
using ECommerceApi.Application.Dtos.Product;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _secretKey = "MY_SUPER_SECRET_KEY_MY_SUPER_SECRET_KEY";
        private readonly AppDbContext _dbContext;
        private readonly PasswordHasherService _passwordHasher;
        private readonly IUserRepository _userRepository;

        public AuthController(AppDbContext context, IUserRepository userRepository)
        {
            _dbContext = context;
            _passwordHasher = new PasswordHasherService();
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            var usersDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                Role = u.Role
            });

            return Ok(usersDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = id,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role
            };

            return Ok(userDto);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = _userRepository.GetUserById(id);

            if (user == null)
                return NotFound();

            await _userRepository.DeleteUser(id);

            return Ok("User deleted successfully.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username or email already exists
            if (await _dbContext.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return BadRequest("Username or email already exists.");
            }

            // Hash the password
            var hashedPassword = _passwordHasher.HashPassword(registerDto.Password);

            // Create the user
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = hashedPassword,
                Email = registerDto.Email,
                Role = string.IsNullOrWhiteSpace(registerDto.Role) ? "User" : registerDto.Role
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok($"User registered successfully. {user.Id}");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user exists in the database
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Verify the password
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate a JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
