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
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound();

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
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound();

            var user = new User
            {
                Id = userDto.Id,
                Username = userDto.Username
            };

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
            var user = _repository.GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl // Assuming UserDto has AvatarUrl
            };

            return Ok(userDto);
        }

        // Upload avatar for user
        [HttpPost("{userId}/upload-avatar")]
        public async Task<IActionResult> UploadAvatar(int userId, [FromForm] IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (avatar.Length > 1048576) // 1MB limit
            {
                return BadRequest("File size exceeds limit (1MB).");
            }

            if (!Directory.Exists(_uploadsFolderPath))
            {
                Directory.CreateDirectory(_uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
            var filePath = Path.Combine(_uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            var user = _repository.GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.AvatarUrl = $"/uploads/{uniqueFileName}";
            _repository.UpdateUser(user);

            return Ok(new { avatarUrl = user.AvatarUrl });
        }

        [HttpGet("avatar/{filename}")]
        public IActionResult GetAvatar(string filename)
        {
            var filePath = Path.Combine(_uploadsFolderPath, filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Avatar not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "image/jpeg"; // Default to jpeg
            if (filename.EndsWith(".png"))
            {
                contentType = "image/png";
            }
            return File(fileBytes, contentType);
        }

        [HttpDelete("{userId}/avatar")]
        public IActionResult DeleteAvatar(int userId)
        {
            var user = _repository.GetUser(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

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
