namespace ChatApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        //public string PasswordHash { get; set; } // Add this property
        public string Password { get; set; }
        // Relationships
        public ICollection<ChatRoom> ChatRooms { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
