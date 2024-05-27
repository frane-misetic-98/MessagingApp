using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}