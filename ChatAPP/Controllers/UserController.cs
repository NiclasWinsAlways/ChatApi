using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Repositories;
using System.Collections.Generic;
using System.Linq;
using ChatAPP.DTO;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IChatRepository _repository;

        public UserController(IChatRepository repository)
        {
            _repository = repository;
        }

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
    }
}
