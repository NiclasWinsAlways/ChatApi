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
        // Injecting the IChatRepository dependency via the constructor
        private readonly IChatRepository _repository;

        public ChatRoomController(IChatRepository repository)
        {
            _repository = repository;
        }

        // Get all chat rooms with their users and messages
        [HttpGet]
        public ActionResult<IEnumerable<ChatRoomDto>> GetChatRooms()
        {
            // Retrieve chat rooms from the repository and map to ChatRoomDto
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

        // Get a specific chat room by its ID
        [HttpGet("{id}")]
        public ActionResult<ChatRoomDto> GetChatRoom(int id)
        {
            // Retrieve the chat room by ID
            var chatRoom = _repository.GetChatRoom(id);
            if (chatRoom == null) return NotFound();

            // Map the chat room data to ChatRoomDto
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

        // Create a new chat room and return the created ChatRoomDto
        [HttpPost]
        public ActionResult<ChatRoomDto> CreateChatRoom(ChatRoom chatRoom)
        {
            // Add the new chat room to the repository
            var createdRoom = _repository.AddChatRoom(chatRoom);

            // Map the created room to ChatRoomDto
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

        // Create a simple chat room with no users or messages
        [HttpPost("create")]
        public ActionResult<ChatRoomDto> CreateSimpleChatRoom(CreateChatRoomDto chatRoomDto)
        {
            // Initialize a new ChatRoom object
            var chatRoom = new ChatRoom
            {
                Name = chatRoomDto.Name,
                Users = new List<User>(),
                Messages = new List<ChatMessage>()
            };

            // Add the chat room to the repository
            var createdRoom = _repository.AddChatRoom(chatRoom);

            // Map the created room to ChatRoomDto
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

        // Get all messages in a specific chat room by chatRoomId
        [HttpGet("{chatRoomId}/messages")]
        public ActionResult<IEnumerable<ChatMessageDto>> GetMessages(int chatRoomId)
        {
            // Retrieve the messages for the given chat room
            var messages = _repository.GetMessages(chatRoomId);
            if (messages == null) return NotFound();

            // Map messages to ChatMessageDto
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
        [HttpPost("{chatRoomId}/messages")]
        public ActionResult<ChatMessageDto> SendMessage(int chatRoomId, [FromBody] CreateMessageDto messageDto)
        {
            // Validate the message data
            if (messageDto == null || string.IsNullOrWhiteSpace(messageDto.Message) || messageDto.UserId <= 0)
            {
                return BadRequest(new { message = "Invalid message data." });
            }

            // Retrieve the user by UserId
            var user = _repository.GetUserById(messageDto.UserId);
            if (user == null) return NotFound(new { message = "User not found." });

            // Retrieve the chat room by chatRoomId
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound(new { message = "Chat room not found." });

            // Create a new ChatMessage object
            var message = new ChatMessage
            {
                Message = messageDto.Message,
                Timestamp = DateTime.Now,
                UserId = messageDto.UserId,
                ChatRoomId = chatRoomId
            };

            // Add the message to the repository
            _repository.AddMessage(message);

            // Map the message to ChatMessageDto
            var messageResponseDto = new ChatMessageDto
            {
                Id = message.Id,
                Message = message.Message,
                Timestamp = message.Timestamp,
                UserId = message.UserId,
                Username = user.Username,
                ChatRoomId = message.ChatRoomId
            };

            // Return the created message using CreatedAtAction
            return CreatedAtAction(
                actionName: nameof(MessageController.GetMessageById),
                controllerName: "Message",
                routeValues: new { chatRoomId = chatRoomId, messageId = message.Id },
                value: messageResponseDto
            );
        }
    }
}
