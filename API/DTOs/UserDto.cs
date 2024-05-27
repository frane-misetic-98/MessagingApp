using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }
}