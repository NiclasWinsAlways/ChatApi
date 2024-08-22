using System;

namespace ChatApp.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty; // Initialize to avoid null reference
        public DateTime Timestamp { get; set; } = DateTime.Now; // Initialize to current date and time
        public int UserId { get; set; }
        public User User { get; set; } = new User(); // Initialize to avoid null reference
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; } = new ChatRoom(); // Initialize to avoid null reference
    }
}
