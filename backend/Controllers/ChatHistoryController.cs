// Controllers/ChatHistoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatBotContext _context;

        public ChatHistoryController(ChatBotContext context)
        {
            _context = context;
        }

        // GET: api/ChatHistory/List
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var sessions = await _context.ChatSessions
                .Where(cs => cs.UserId == userId)
                .Select(cs => new
                {
                    id = cs.Id,
                    title = cs.Title ?? $"Chat {cs.Id}",
                    lastUpdated = cs.LastUpdated,
                    lastMessage = cs.Messages.OrderByDescending(m => m.Timestamp).Select(m => m.Content).FirstOrDefault() ?? "",
                    MessageCount = cs.Messages.Count
                })
                .OrderByDescending(cs => cs.lastUpdated)
                .ToListAsync();

            return Ok(sessions);
        }

        // GET: api/ChatHistory/{sessionId}
        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var session = await _context.ChatSessions
                .Include(cs => cs.Messages.OrderBy(m => m.Timestamp))
                .FirstOrDefaultAsync(cs => cs.Id == sessionId && cs.UserId == userId);

            if (session == null) return NotFound();

            var messages = session.Messages.Select(m => new
            {
                role = m.Role,
                content = m.Content,
            });

            return Ok(messages);
        }
    }
}
