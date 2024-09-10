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

        // Get all chat rooms
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

        // Get a specific chat room by ID
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

        // Create a new chat room
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

        // Create a simple chat room
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

        // Get all messages for a specific chat room
        [HttpGet("{chatRoomId}/messages")]
        public ActionResult<IEnumerable<ChatMessageDto>> GetMessages(int chatRoomId)
        {
            var messages = _repository.GetMessages(chatRoomId);
            if (messages == null) return NotFound();

            var messageDtos = messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                Message = m.Message,
                Timestamp = m.Timestamp,
                UserId = m.UserId,
                Username = m.User.Username,
                ChatRoomId = m.ChatRoomId
            }).ToList();

            return Ok(messageDtos);
        }

        // Send a message in a specific chat room
        // Send a message in a specific chat room
        [HttpPost("{chatRoomId}/messages")]
        public ActionResult<ChatMessageDto> SendMessage(int chatRoomId, [FromBody] CreateMessageDto messageDto)
        {
            if (messageDto == null || string.IsNullOrWhiteSpace(messageDto.Message) || messageDto.UserId <= 0)
            {
                return BadRequest(new { message = "Invalid message data." });
            }

            var user = _repository.GetUserById(messageDto.UserId);
            if (user == null) return NotFound(new { message = "User not found." });

            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound(new { message = "Chat room not found." });

            var message = new ChatMessage
            {
                Message = messageDto.Message,
                Timestamp = DateTime.Now,
                UserId = messageDto.UserId,
                ChatRoomId = chatRoomId
            };

            _repository.AddMessage(message);

            var messageResponseDto = new ChatMessageDto
            {
                Id = message.Id,
                Message = message.Message,
                Timestamp = message.Timestamp,
                UserId = message.UserId,
                Username = user.Username,
                ChatRoomId = message.ChatRoomId
            };

            // Use nameof with controller specified
            return CreatedAtAction(
                actionName: nameof(MessageController.GetMessageById),
                controllerName: "Message",
                routeValues: new { chatRoomId = chatRoomId, messageId = message.Id },
                value: messageResponseDto
            );
        }

    }
}
