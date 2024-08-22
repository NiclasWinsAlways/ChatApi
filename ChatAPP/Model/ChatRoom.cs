using System.Collections.Generic;

namespace ChatApp.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Initialize to avoid null reference
        public ICollection<User> Users { get; set; } = new List<User>(); // Initialize to avoid null reference
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>(); // Initialize to avoid null reference
    }
}
