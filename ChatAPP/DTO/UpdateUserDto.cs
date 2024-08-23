namespace ChatAPP.DTO
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }  // Nullable to allow optional updates
        public string? Email { get; set; }     // Nullable to allow optional updates
        public string? Password { get; set; }  // Nullable to allow optional updates
        public string? Role { get; set; }      // Nullable if you want to allow updating the role as well
    }
}
