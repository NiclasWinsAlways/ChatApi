using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatAPP.DTO;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IChatRepository _repository;

        public AuthController(IChatRepository repository)
        {
            _repository = repository;
        }

        // Register a new user without password hashing
        [HttpPost("register")]
        public ActionResult Register(RegisterUserDto registerDto)
        {
            var existingUser = _repository.GetUserByUsername(registerDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password // Store the password directly
            };

            _repository.AddUser(user);

            return Ok("User registered successfully.");
        }

        // Login a user without password hashing
        [HttpPost("login")]
        public ActionResult Login(LoginUserDto loginDto)
        {
            var user = _repository.GetUserByUsername(loginDto.Username);
            if (user == null || user.Password != loginDto.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful.");
        }

        // Get all users
        [HttpGet("accounts")]
        public ActionResult<IEnumerable<UserDto>> GetAccounts()
        {
            var users = _repository.GetAllUsers()
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username
                }).ToList();

            return Ok(users);
        }

        // Get a user by ID
        [HttpGet("accounts/{id}")]
        public ActionResult<UserDto> GetAccountById(int id)
        {
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };

            return Ok(userDto);
        }

        // Update a user by ID
        [HttpPut("accounts/{id}")]
        public ActionResult UpdateAccount(int id, UpdateUserDto updateUserDto)
        {
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user properties
            user.Username = updateUserDto.Username;
            user.Password = updateUserDto.Password;

            _repository.UpdateUser(user);

            return Ok("User updated successfully.");
        }

        // Delete a user by ID
        [HttpDelete("accounts/{id}")]
        public ActionResult DeleteAccount(int id)
        {
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _repository.DeleteUser(id);

            return Ok("User deleted successfully.");
        }
    }
}
