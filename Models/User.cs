using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]   // sẽ map thành VARCHAR(100)
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
    }
}
