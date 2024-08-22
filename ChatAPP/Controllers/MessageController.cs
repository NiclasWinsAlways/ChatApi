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

        // This method should ideally be in ChatRoomController, but if it needs to be here:
      
    }
}
