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
    public class MessageController : ControllerBase
    {
        private readonly IChatRepository _repository;

        public MessageController(IChatRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{chatRoomId}/messages")]
        public ActionResult<IEnumerable<ChatMessageDto>> GetMessagesInChatRoom(int chatRoomId)
        {
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound();

            var messages = chatRoom.Messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                Message = m.Message,
                Timestamp = m.Timestamp,
                UserId = m.UserId,
                Username = m.User.Username,
                ChatRoomId = m.ChatRoomId
            }).ToList();

            return Ok(messages);
        }

        [HttpPost("{chatRoomId}/messages")]
        public ActionResult<ChatMessageDto> AddMessageToChatRoom(int chatRoomId, ChatMessageDto messageDto)
        {
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound();

            var user = _repository.GetUser(messageDto.UserId);
            if (user == null) return NotFound();

            var message = new ChatMessage
            {
                Message = messageDto.Message,
                Timestamp = messageDto.Timestamp,
                UserId = messageDto.UserId,
                User = user,
                ChatRoomId = chatRoomId,
                ChatRoom = chatRoom
            };

            _repository.AddMessage(message);

            messageDto.Id = message.Id; // Return the generated Id

            return CreatedAtAction(nameof(GetMessagesInChatRoom), new { chatRoomId }, messageDto);
        }

        // Get a specific message by ID
        [HttpGet("{chatRoomId}/messages/{messageId}")]
        public ActionResult<ChatMessageDto> GetMessageById(int chatRoomId, int messageId)
        {
            var message = _repository.GetMessageById(messageId);
            if (message == null || message.ChatRoomId != chatRoomId) return NotFound();

            var messageDto = new ChatMessageDto
            {
                Id = message.Id,
                Message = message.Message,
                Timestamp = message.Timestamp,
                UserId = message.UserId,
                Username = message.User.Username,
                ChatRoomId = message.ChatRoomId
            };

            return Ok(messageDto);
        }

        // Update a message
        [HttpPut("{chatRoomId}/messages/{messageId}")]
        public ActionResult UpdateMessage(int chatRoomId, int messageId, ChatMessageDto messageDto)
        {
            var message = _repository.GetMessageById(messageId);
            if (message == null || message.ChatRoomId != chatRoomId) return NotFound();

            message.Message = messageDto.Message;
            message.Timestamp = messageDto.Timestamp;

            _repository.UpdateMessage(message);

            return Ok("Message updated successfully.");
        }

        // Delete a message
        [HttpDelete("{chatRoomId}/messages/{messageId}")]
        public ActionResult DeleteMessage(int chatRoomId, int messageId)
        {
            var message = _repository.GetMessageById(messageId);
            if (message == null || message.ChatRoomId != chatRoomId) return NotFound();

            _repository.DeleteMessage(messageId);

            return Ok("Message deleted successfully.");
        }
    }
}
