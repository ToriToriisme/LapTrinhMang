using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Auth
{
    public class LoginRequest
    {
        [Required, MaxLength(100)]
        public string Username { get; set; } = "";

        [Required, MaxLength(100)]
        public string Password { get; set; } = "";
    }
}
