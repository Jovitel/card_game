using CardGame.Server.Data;
using CardGame.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Api.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public StatsController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("win/{userId}")]
        public async Task<IActionResult> AddWin(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            user.Wins++;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("loss/{userId}")]
        public async Task<IActionResult> AddLoss(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            user.Losses++;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}