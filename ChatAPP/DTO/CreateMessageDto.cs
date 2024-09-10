using System.ComponentModel.DataAnnotations;

namespace ChatAPP.DTO
{
    public class CreateMessageDto
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public int UserId { get; set; }
    }

}
