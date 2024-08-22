namespace ChatAPP.DTO
{
    public class ChatRoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserDto> Users { get; set; }
        public List<ChatMessageDto> Messages { get; set; }
    }

}
