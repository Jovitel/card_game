using CardGame.Server.Data;
using CardGame.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UsersController(UserDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName == login.UserName && u.Password == login.Password);

            if (user == null) return Unauthorized();
            return Ok(user);
        }
        [HttpGet("byname/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                // Grąžiname 404, jei vartotojas nerastas
                return NotFound("Vartotojas nerastas.");
            }

            // Grąžiname visą User objektą
            return user;
        }
        [HttpPost("addxp/{userId}/{amount}")]
        public async Task<IActionResult> AddXp(int userId, int amount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.ExperiencePoints += amount;

            int xpRequired = GetXpForNextLevel(user.Level);
            while (user.ExperiencePoints >= xpRequired)
            {
                user.Level++;
                xpRequired = GetXpForNextLevel(user.Level);
            }


            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // Naujas pagalbinis metodas:
        private static int GetXpForNextLevel(int level)
        {
            return (int)(100 * Math.Pow(level, 1.5)) + 150;
        }


        [HttpPost("updateavatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] AvatarUpdateModel model)
        {
            if (model == null || model.UserId <= 0)
            {
                return BadRequest("Neteisingi duomenys.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return NotFound("Vartotojas nerastas.");
            }

            // Atnaujiname avataro URL ir įrašome į DB
            user.AvatarUrl = model.AvatarUrl;
            await _context.SaveChangesAsync();

            return Ok();
        }
        public class AvatarUpdateModel
        {
            public int UserId { get; set; }
            public string AvatarUrl { get; set; } = string.Empty;
        }
        [HttpPut("updateusername")]
        public async Task<IActionResult> UpdateUserName([FromBody] UsernameUpdateModel model)
        {
            if (model == null || model.UserId <= 0 || string.IsNullOrWhiteSpace(model.NewUserName))
            {
                return BadRequest("Neteisingi duomenys: trūksta ID arba naujo vardo.");
            }

            var newUsername = model.NewUserName.Trim();

            // 1. Patikrinimas: Ar vardas jau užimtas?
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == newUsername && u.Id != model.UserId);

            if (existingUser != null)
            {
                // Grąžiname 409 Conflict, kad Klientas (Blazor) žinotų, jog vardas užimtas.
                return Conflict("Šis vartotojo vardas jau yra užimtas.");
            }

            // 2. Atnaujinimas: Surandame vartotoją, kurį reikia atnaujinti
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (userToUpdate == null)
            {
                return NotFound("Vartotojas nerastas.");
            }

            // 3. Atnaujiname vardą ir išsaugome DB
            userToUpdate.UserName = newUsername;
            await _context.SaveChangesAsync();

            // Grąžiname sėkmės pranešimą (200 OK)
            return Ok(userToUpdate);
        }
        [HttpPost("updatesetting")]
        public async Task<IActionResult> UpdateUserSetting([FromBody] UpdateSettingRequest request)
        {
            // Rasti vartotoją pagal ID
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Atnaujinti atitinkamą lauką
            switch (request.SettingType)
            {
                case "CardColor":
                    user.SelectedCardColor = request.Value;
                    break;
                case "TableColor":
                    user.SelectedTableColor = request.Value;
                    break;
                case "AvatarUrl": // Jei norite naudoti ir avataro atnaujinimui
                    user.AvatarUrl = request.Value;
                    break;
                default:
                    return BadRequest("Neteisingas nustatymų tipas.");
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("settheme/{id}/{theme}")]
        public async Task<IActionResult> SetTheme(int id, string theme)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (theme != "dark" && theme != "light")
                return BadRequest("Neteisinga temos reikšmė.");

            user.ThemePreference = theme;
            await _context.SaveChangesAsync();

            return Ok();
        }



    }
    public class UsernameUpdateModel
    {
        public int UserId { get; set; }
        public string NewUserName { get; set; } = string.Empty;
    }
    // Reikalingas modelis duomenims priimti (turėtų būti CardGame.Shared)
    public class UpdateSettingRequest
    {
        public int UserId { get; set; }
        public string SettingType { get; set; }
        public string Value { get; set; }
    }
}
