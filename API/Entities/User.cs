using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        [Required]
        public List<Message> MessagesSent { get; set; } = [];
        [Required]
        public List<Message> MessagesRecieved { get; set; } = [];
    }
}