using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config) => _config = config;

        public string GenerateToken(User user)
        {
            var keyString = _config.GetValue<string>("Jwt:Key")
                ?? throw new InvalidOperationException("Missing Jwt:Key in appsettings.json");

            var issuer         = _config.GetValue<string>("Jwt:Issuer") ?? "BlogApi";
            var audience       = _config.GetValue<string>("Jwt:Audience") ?? issuer;
            var expiresMinutes = _config.GetValue<int?>("Jwt:ExpiresMinutes") ?? 60;

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("id", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
