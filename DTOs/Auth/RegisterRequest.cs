using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required, MaxLength(100)]
        public string Username { get; set; } = "";

        [Required, MaxLength(100)]
        public string Password { get; set; } = "";

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = "";
    }
}
