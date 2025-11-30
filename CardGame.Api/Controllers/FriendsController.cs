using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardGame.Shared.Models;
using CardGame.Server.Data; // tavo UserDbContext vieta

namespace CardGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public FriendsController(UserDbContext context)
        {
            _context = context;
        }

        // Ieško vartotojų pagal vardą
        [HttpGet("search/{query}")]
        public async Task<ActionResult<IEnumerable<object>>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<object>());

            var results = await _context.Users
                .Where(u => u.UserName.Contains(query))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Wins,
                    u.Losses
                })
                .ToListAsync();

            return Ok(results);
        }

        // Prideda draugą (abiem pusėm)
        [HttpPost("{userId}/add/{friendId}")]
        public async Task<IActionResult> AddFriend(int userId, int friendId)
        {
            if (userId == friendId)
                return BadRequest("Negalima pridėti savęs.");

            var user = await _context.Users.FindAsync(userId);
            var friend = await _context.Users.FindAsync(friendId);
            if (user == null || friend == null)
                return NotFound("Vartotojas nerastas.");

            user.Friends ??= new List<int>();
            friend.Friends ??= new List<int>();

            if (!user.Friends.Contains(friendId))
                user.Friends.Add(friendId);

            if (!friend.Friends.Contains(userId))
                friend.Friends.Add(userId);

            // 👇 Pridedam šiuos du EF "notify" kvietimus:
            _context.Entry(user).Property(u => u.Friends).IsModified = true;
            _context.Entry(friend).Property(u => u.Friends).IsModified = true;

            await _context.SaveChangesAsync();

            Console.WriteLine("[DEBUG] Pokyčiai išsaugoti DB (su IsModified)");
            return Ok("Draugas pridėtas sėkmingai.");
        }


        // Gauti draugų sąrašą
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFriends(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Vartotojas nerastas.");

            user.Friends ??= new List<int>();

            var friends = await _context.Users
                .Where(u => user.Friends.Contains(u.Id))
                .OrderByDescending(u => u.Wins)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Wins,
                    u.Losses
                })
                .ToListAsync();

            return Ok(friends);
        }
    }
}
