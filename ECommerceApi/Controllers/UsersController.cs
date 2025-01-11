using ECommerceApi.Application.Dtos.Auth;
using ECommerceApi.Application.Dtos.Product;
using ECommerceApi.Application.Dtos.User;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
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
                Address = u.Address,
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
                Address = user.Address,
                Role = user.Role
            };

            return Ok(userDto);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst("sub")?.Value; // Extract the user's ID from the token
            var isAdmin = User.IsInRole("Admin"); // Check if the user is an admin

            if (!isAdmin && userIdClaim != id.ToString())
            {
                // Non-admins can only update their own user data
                return Forbid();
            }

            if (!isAdmin && userDto.Role != null)
            {
                // Non-admins cannot update the role
                return BadRequest(new { message = "You are not allowed to change roles." });
            }

            var user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            // Update the fields
            user.Username = userDto.Username ?? user.Username;
            user.Email = userDto.Email ?? user.Email;

            if (isAdmin && userDto.Role != null)
            {
                user.Role = userDto.Role;
            }

            await _userRepository.UpdateUser(user);

            return Ok();
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
    }
}
