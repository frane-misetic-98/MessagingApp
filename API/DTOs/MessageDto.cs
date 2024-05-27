using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class MessageDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public string SenderUsername { get; set; }
        [Required]
        public int RecipientId { get; set; }
        [Required]
        public string RecipientUsername { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime MessageSent { get; set; }
        public DateTime? DateRead { get; set; }
    }
}