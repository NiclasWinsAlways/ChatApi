namespace ChatApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; } // This can be optional or removed if you're using email only

        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "User"; // Default role is User

        // Relationships
        public ICollection<ChatRoom> ChatRooms { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
