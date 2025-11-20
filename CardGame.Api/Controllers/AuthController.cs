using Microsoft.AspNetCore.Mvc;
using CardGame.Shared.Models;

namespace CardGame.Shared.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Paprastas sąrašas vartotojų (kol kas laikom atmintyje)
        private static List<User> Users = new List<User>();

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("User with this email already exists!");
            }

            Users.Add(user);
            return Ok(new { message = "Registration successful", user.UserName });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var existingUser = Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (existingUser == null)
                return Unauthorized("Invalid credentials");

            return Ok(existingUser); // <-- grąžiname visą User objektą
        }
    }
}