using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using Blog.Services;
using Blog.Data;
using BlogApp_SharedModels.Models;
using Microsoft.AspNetCore.Identity;
using Blog.DTOs;
using Newtonsoft.Json.Linq;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/Path")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDBContext _dbContext;

        public AuthController(AuthService authService, AppDBContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            return passwordHasher.HashPassword(new IdentityUser(), password);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var result = passwordHasher.VerifyHashedPassword(new IdentityUser(), storedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            if (_dbContext.users.Any(u => u.email == model.email))
                return BadRequest(new {message = "El email ingresado ya está registrado..."});

            var user = new User
            {
                email = model.email,
                name = model.name,
                passwordhash = HashPassword(model.passwordhash),
                createdat = DateTime.UtcNow,
                profilepicture = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(model.name)}",
            };

            _dbContext.users.Add(user);
            _dbContext.SaveChanges();

            return Ok(new {message = "Usuario registrado con éxito"});
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            var user = _dbContext.users.FirstOrDefault(u => u.email == model.email);
            if (user == null)
                return BadRequest(new { message = "Usuario no encontrado" });
            if (!VerifyPassword(model.passwordhash, user.passwordhash))
                return BadRequest(new {message = "Contraseña incorrecta"});

            var Token = _authService.GenerateJwtToken(user);
            return Ok(new
            {
                Token,
                user = new
                {
                    id = user.id,
                    name = user.name,
                    email = user.email,
                    profilepicture = user.profilepicture
                }
            });
        }
    }
}
