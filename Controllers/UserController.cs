using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Fake user data (sau này sẽ lưu DB thật)
        private static readonly string demoUsername = "admin";
        private static readonly string demoPassword = "123456";
        private static bool isLoggedIn = false;

        // POST: api/user/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == demoUsername && request.Password == demoPassword)
            {
                isLoggedIn = true;
                return Ok(new { message = "Login successful!" });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }

        // POST: api/user/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (isLoggedIn)
            {
                isLoggedIn = false;
                return Ok(new { message = "Logged out successfully!" });
            }

            return BadRequest(new { message = "User not logged in" });
        }
    }

    // Request model
    public class LoginRequest
    {
        // ✅ Cho phép null hoặc gán mặc định
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
