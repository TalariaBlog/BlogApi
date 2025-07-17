using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApp_SharedModels.Models;

namespace Blog.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user)
        {
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var Claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Email, user.email),
            };

            var Token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: Credentials);

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
