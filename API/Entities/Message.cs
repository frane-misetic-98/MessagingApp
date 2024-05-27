using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public User Sender { get; set; }
        [Required]
        public int RecipientId { get; set; }
        [Required]
        public User Recipient { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public DateTime? DateRead { get; set; }
    }
}