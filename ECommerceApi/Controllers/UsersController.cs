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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetUserById(id);

            if (user == null) return NotFound();

            if(userDto.Email != null)
            {
                user.Email = userDto.Email;
            }

            if(userDto.Username != null)
            {
                user.Username = userDto.Username;
            }

            if (userDto.Role != null)
            {
                user.Role = userDto.Role;
            }

            await _userRepository.UpdateUser(user);

            return NoContent();
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
