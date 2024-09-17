namespace ChatAPP.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } // Optional
        public string Email { get; set; }
        public string Role { get; set; }

        //add Avatar URL to store AvatarURLs
        public string AvatarUrl { get; set; }
    }
}