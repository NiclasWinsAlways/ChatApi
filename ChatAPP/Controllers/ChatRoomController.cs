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
    public class ChatRoomController : ControllerBase
    {
        private readonly IChatRepository _repository;

        public ChatRoomController(IChatRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ChatRoomDto>> GetChatRooms()
        {
            var chatRooms = _repository.GetChatRooms()
                .Select(r => new ChatRoomDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Users = r.Users.Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username
                    }).ToList(),
                    Messages = r.Messages.Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        Message = m.Message,
                        Timestamp = m.Timestamp,
                        UserId = m.UserId,
                        Username = m.User.Username,
                        ChatRoomId = m.ChatRoomId
                    }).ToList()
                }).ToList();

            return Ok(chatRooms);
        }

        [HttpGet("{id}")]
        public ActionResult<ChatRoomDto> GetChatRoom(int id)
        {
            var chatRoom = _repository.GetChatRoom(id);
            if (chatRoom == null) return NotFound();

            var chatRoomDto = new ChatRoomDto
            {
                Id = chatRoom.Id,
                Name = chatRoom.Name,
                Users = chatRoom.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username
                }).ToList(),
                Messages = chatRoom.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    Timestamp = m.Timestamp,
                    UserId = m.UserId,
                    Username = m.User.Username,
                    ChatRoomId = m.ChatRoomId
                }).ToList()
            };

            return Ok(chatRoomDto);
        }

        [HttpPost]
        public ActionResult<ChatRoomDto> CreateChatRoom(ChatRoom chatRoom)
        {
            var createdRoom = _repository.AddChatRoom(chatRoom);

            var chatRoomDto = new ChatRoomDto
            {
                Id = createdRoom.Id,
                Name = createdRoom.Name,
                Users = createdRoom.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username
                }).ToList(),
                Messages = createdRoom.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    Timestamp = m.Timestamp,
                    UserId = m.UserId,
                    Username = m.User.Username,
                    ChatRoomId = m.ChatRoomId
                }).ToList()
            };

            return CreatedAtAction(nameof(GetChatRoom), new { id = createdRoom.Id }, chatRoomDto);
        }

        [HttpPost("create")]
        public ActionResult<ChatRoomDto> CreateSimpleChatRoom(CreateChatRoomDto chatRoomDto)
        {
            var chatRoom = new ChatRoom
            {
                Name = chatRoomDto.Name,
                Users = new List<User>(),
                Messages = new List<ChatMessage>()
            };

            var createdRoom = _repository.AddChatRoom(chatRoom);

            var responseDto = new ChatRoomDto
            {
                Id = createdRoom.Id,
                Name = createdRoom.Name,
                Users = createdRoom.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username
                }).ToList(),
                Messages = createdRoom.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    Timestamp = m.Timestamp,
                    UserId = m.UserId,
                    Username = m.User.Username,
                    ChatRoomId = m.ChatRoomId
                }).ToList()
            };

            return CreatedAtAction(nameof(GetChatRoom), new { id = createdRoom.Id }, responseDto);
        }
    }
}
