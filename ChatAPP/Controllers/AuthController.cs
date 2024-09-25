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
        // Injecting the IChatRepository dependency via the constructor
        private readonly IChatRepository _repository;

        public AuthController(IChatRepository repository)
        {
            _repository = repository;
        }

        // Endpoint to register a new user
        [HttpPost("register")]
        public ActionResult Register(RegisterUserDto registerDto)
        {
            // Check if email is provided and valid
            if (string.IsNullOrEmpty(registerDto.Email))
            {
                return BadRequest("Email is required.");
            }

            // Check if the email is already registered
            var existingUser = _repository.GetUserByEmail(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already registered.");
            }

            // Create a new User object
            var user = new User
            {
                Username = string.IsNullOrEmpty(registerDto.Username) ? null : registerDto.Username,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Role = "User"  // Default role is set to "User"
            };

            // Save the user to the database
            _repository.AddUser(user);

            return Ok(new { userId = user.Id, message = "User registered successfully." });
        }

        // Endpoint to promote a user to admin
        [HttpPost("promote/{id}")]
        public ActionResult PromoteToAdmin(int id)
        {
            // Fetch user by their ID
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update the user's role to Admin
            user.Role = "Admin";
            _repository.UpdateUser(user);

            return Ok("User promoted to admin successfully.");
        }

        // Endpoint to login a user
        [HttpPost("login")]
        public ActionResult Login(LoginUserDto loginDto)
        {
            // Retrieve the user from the repository by email
            var user = _repository.GetUserByEmail(loginDto.Email);

            // Check if the user exists and the password matches
            if (user == null || user.Password != loginDto.Password)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Return a success message along with user details
            return Ok(new
            {
                userId = user.Id,
                username = user.Username,  // Retrieve the username from the user object
                message = "Login successful.",
                isAdmin = user.Role == "Admin"
            });
        }

        // Endpoint to get all users
        [HttpGet("accounts")]
        public ActionResult<IEnumerable<UserDto>> GetAccounts()
        {
            // Fetch all users from the repository
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

        // Endpoint to get a user by ID
        [HttpGet("accounts/{id}")]
        public ActionResult<UserDto> GetAccountById(int id)
        {
            // Fetch user by ID
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Map user to UserDto
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDto);
        }

        // Endpoint to update a user by ID
        [HttpPut("accounts/{id}")]
        public ActionResult UpdateAccount(int id, UpdateUserDto updateUserDto)
        {
            // Fetch user by ID
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user properties only if provided
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

            // Update the user in the repository
            _repository.UpdateUser(user);

            return Ok("User updated successfully.");
        }

        // Endpoint to delete a user by ID
        [HttpDelete("accounts/{id}")]
        public ActionResult DeleteAccount(int id)
        {
            // Fetch user by ID
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Delete the user from the repository
            _repository.DeleteUser(id);

            return Ok("User deleted successfully.");
        }


    }
}
