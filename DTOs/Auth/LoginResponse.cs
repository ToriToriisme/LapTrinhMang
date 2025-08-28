namespace BlogApi.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
