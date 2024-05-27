using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Responses
{
    public class LoginResponse
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Token { get; set; }
    }
}