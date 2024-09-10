using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Repositories;
using System;
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

        // Get all messages in a chat room
        [HttpGet("{chatRoomId}/messages")]
        public ActionResult<IEnumerable<ChatMessageDto>> GetMessagesInChatRoom(int chatRoomId)
        {
            var chatRoom = _repository.GetChatRoom(chatRoomId);
            if (chatRoom == null) return NotFound(new { error = "Chat room not found" });

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

        // Add a message to a chat room
        [HttpPost("{chatRoomId}/messages")]
        public ActionResult<ChatMessageDto> AddMessageToChatRoom(int chatRoomId, ChatMessageDto messageDto)
        {
            try
            {
                var chatRoom = _repository.GetChatRoom(chatRoomId);
                if (chatRoom == null) return NotFound(new { error = "Chat room not found" });

                var user = _repository.GetUser(messageDto.UserId);
                if (user == null) return NotFound(new { error = "User not found" });

                var message = new ChatMessage
                {
                    Message = messageDto.Message,
                    Timestamp = messageDto.Timestamp == default ? DateTime.UtcNow : messageDto.Timestamp,
                    UserId = messageDto.UserId,
                    ChatRoomId = chatRoomId,
                    ChatRoom = chatRoom,
                    User = user
                };

                _repository.AddMessage(message);

                messageDto.Id = message.Id;
                messageDto.Timestamp = message.Timestamp;

                return CreatedAtAction(nameof(GetMessageById), new { chatRoomId = chatRoomId, messageId = message.Id }, messageDto);
            }
            catch (Exception ex)
            {
                // Log the exception to understand where the error is occurring
                Console.WriteLine(ex);

                // Return a JSON response with a 500 error code and a custom error message
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
        }



        // Get a specific message by ID
        [HttpGet("{chatRoomId}/messages/{messageId}")]
        public ActionResult<ChatMessageDto> GetMessageById(int chatRoomId, int messageId)
        {
            var message = _repository.GetMessageById(messageId);
            if (message == null || message.ChatRoomId != chatRoomId) return NotFound(new { error = "Message not found" });

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
            if (message == null || message.ChatRoomId != chatRoomId) return NotFound(new { error = "Message not found or does not belong to the chat room" });

            message.Message = messageDto.Message;
            message.Timestamp = messageDto.Timestamp == default ? DateTime.UtcNow : messageDto.Timestamp; // Update timestamp if provided

            _repository.UpdateMessage(message);

            return Ok(new { message = "Message updated successfully" });
        }

        // Delete a message
        [HttpDelete("{chatRoomId}/messages/{messageId}")]
        public ActionResult DeleteMessage(int chatRoomId, int messageId)
        {
            var message = _repository.GetMessageById(messageId);
            if (message == null || message.ChatRoomId != chatRoomId) return NotFound(new { error = "Message not found or does not belong to the chat room" });

            _repository.DeleteMessage(messageId);

            return Ok(new { message = "Message deleted successfully" });
        }
    }
}
