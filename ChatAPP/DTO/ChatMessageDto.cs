namespace ChatAPP.DTO
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } // Simplified to show only the username
        public int ChatRoomId { get; set; }
    }

}
