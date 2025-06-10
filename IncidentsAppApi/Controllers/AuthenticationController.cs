using IncidentsAppApi.Database;
using IncidentsAppApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IncidentsAppApi.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyDbContext _context;

        public AuthenticationController(IConfiguration config, MyDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("api/login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var users = _context.Users.Where(u =>
                u.Username == login.Username);

            if (!users.Any())
            {
                return NotFound();
            }
            User user = users.First();

            string decryptedPassword = Encryptor.Decrypt(user.Password);

            if (decryptedPassword != login.Password)
            {
                return BadRequest();
            }
            return Ok(new { username = user.Username, isAdmin = user.IsAdmin });
        }
    }
}
