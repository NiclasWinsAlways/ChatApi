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

        // Register a new user with email
        [HttpPost("register")]
        public ActionResult Register(RegisterUserDto registerDto)
        {
            var existingUser = _repository.GetUserByEmail(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already registered.");
            }

            var user = new User
            {
                Username = string.IsNullOrEmpty(registerDto.Username) ? null : registerDto.Username,  // Handle nullable username
                Email = registerDto.Email,
                Password = registerDto.Password,
                Role = "User"
            };

            _repository.AddUser(user);

            return Ok("User registered successfully.");
        }

        // Promote a user to admin
        [HttpPost("promote/{id}")]
        public ActionResult PromoteToAdmin(int id)
        {
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Role = "Admin";
            _repository.UpdateUser(user);

            return Ok("User promoted to admin successfully.");
        }

        // Login a user with email
        [HttpPost("login")]
        public ActionResult Login(LoginUserDto loginDto)
        {
            var user = _repository.GetUserByEmail(loginDto.Email);
            if (user == null || user.Password != loginDto.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Check if the user is an admin
            bool isAdmin = user.Role == "Admin";

            return Ok(new { message = "Login successful.", isAdmin });
        }

        // Get all users
        [HttpGet("accounts")]
        public ActionResult<IEnumerable<UserDto>> GetAccounts()
        {
            var users = _repository.GetAllUsers()
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username, // Ensure Username is displayed
                    Email = user.Email,
                    Role = user.Role
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
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
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

            // Update user properties only if they are provided
            if (!string.IsNullOrEmpty(updateUserDto.Username))
            {
                user.Username = updateUserDto.Username;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                // Check if the email is already taken by another user
                var existingUser = _repository.GetUserByEmail(updateUserDto.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    return BadRequest("Email already in use by another user.");
                }

                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.Password = updateUserDto.Password;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Role))
            {
                user.Role = updateUserDto.Role;
            }

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
