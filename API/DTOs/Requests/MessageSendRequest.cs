using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests
{
    public class MessageSendRequest
    {
        [Required]
        public int RecipientId { get; set; }
        [Required]
        public string Content { get; set; }
    }
}