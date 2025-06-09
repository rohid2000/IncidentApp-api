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
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly IConfiguration _config;
        private readonly MyDbContext _context;

        public AuthenticationController(IConfiguration config, MyDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> Login([FromForm] LoginModel login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == login.Username);

            //if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            //{
            //    return Unauthorized("Invalid credentials!");
            //}

            return Ok();
        }
    }
}
