using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ChatAPP.DTO;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // Injecting the IChatRepository dependency and setting the uploads folder path
        private readonly IChatRepository _repository;
        private readonly string _uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public UserController(IChatRepository repository)
        {
            _repository = repository;
        }

        // --------------------------------------
        // Chat Room Operations
        // --------------------------------------

        // Get all users in a chat room
        [HttpGet("{chatRoomId}/users")]
        public ActionResult<IEnumerable<UserDto>> GetUsersInChatRoom(int chatRoomId)
        {
            // Retrieve the chat room from the repository
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound();

            // Map the users in the chat room to a list of UserDto objects
            var users = chatRoom.Users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username
            }).ToList();

            return Ok(users);
        }

        // Add a user to a chat room
        [HttpPost("{chatRoomId}/users")]
        public ActionResult AddUserToChatRoom(int chatRoomId, UserDto userDto)
        {
            // Retrieve the chat room from the repository
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound();

            // Create a new user object from the provided UserDto
            var user = new User
            {
                Id = userDto.Id,
                Username = userDto.Username
            };

            // Add the user to the chat room
            _repository.AddUserToRoom(chatRoomId, user);
            return NoContent();
        }

        // --------------------------------------
        // Profile and Avatar Management
        // --------------------------------------

        // Get user profile (including avatar)
        [HttpGet("{userId}/profile")]
        public ActionResult<UserDto> GetUserProfile(int userId)
        {
            // Retrieve the user from the repository by their ID
            var user = _repository.GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Map user data to UserDto, including the avatar URL
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl // Assuming UserDto has AvatarUrl
            };

            return Ok(userDto);
        }

        // Upload avatar for a user
        [HttpPost("{userId}/upload-avatar")]
        public async Task<IActionResult> UploadAvatar(int userId, [FromForm] IFormFile avatar)
        {
            // Check if a file was uploaded and validate the file size
            if (avatar == null || avatar.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (avatar.Length > 1048576) // 1MB limit
            {
                return BadRequest("File size exceeds limit (1MB).");
            }

            // Ensure the uploads directory exists, create if not
            if (!Directory.Exists(_uploadsFolderPath))
            {
                Directory.CreateDirectory(_uploadsFolderPath);
            }

            // Generate a unique filename and save the file to the uploads folder
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
            var filePath = Path.Combine(_uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            // Retrieve the user and update their AvatarUrl property
            var user = _repository.GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.AvatarUrl = $"/uploads/{uniqueFileName}";
            _repository.UpdateUser(user);

            return Ok(new { avatarUrl = user.AvatarUrl });
        }

        // Get the avatar image by filename
        [HttpGet("avatar/{filename}")]
        public IActionResult GetAvatar(string filename)
        {
            // Get the avatar file path
            var filePath = Path.Combine(_uploadsFolderPath, filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Avatar not found.");
            }

            // Return the file as a response, with the appropriate content type
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "image/jpeg"; // Default to jpeg
            if (filename.EndsWith(".png"))
            {
                contentType = "image/png";
            }
            return File(fileBytes, contentType);
        }

        // Delete a user's avatar
        [HttpDelete("{userId}/avatar")]
        public IActionResult DeleteAvatar(int userId)
        {
            // Retrieve the user by ID
            var user = _repository.GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // If the user has an avatar, delete the file and clear the AvatarUrl
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var filePath = Path.Combine(_uploadsFolderPath, Path.GetFileName(user.AvatarUrl));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                user.AvatarUrl = null;
                _repository.UpdateUser(user);
            }

            return NoContent();
        }

    }
}
