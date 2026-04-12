using Messenger.Data;
using MessengerIPZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MessengerIPZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class SendMessageDto
        {
            public Guid ChannelId { get; set; }
            public string Content { get; set; }
        }

        // Відправка повідомлення в чат
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Перевіряємо, чи має юзер право писати в цей чат (чи є він учасником)
            var isMember = await _context.ChannelMembers
                .AnyAsync(cm => cm.ChannelId == model.ChannelId && cm.UserId == user.Id);

            if (!isMember) return StatusCode(403, "Ви не є учасником цього чату і не можете писати сюди.");

            var message = new Message
            {
                ChannelId = model.ChannelId,
                UserId = user.Id,
                Content = model.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message.Id,
                message.Content,
                message.Timestamp,
                Sender = user.UserName
            });
        }

        // Отримання всіх повідомлень конкретного чату
        [HttpGet("{channelId}")]
        public async Task<IActionResult> GetMessages(Guid channelId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Перевіряємо, чи має юзер право читати цей чат
            var isMember = await _context.ChannelMembers
                .AnyAsync(cm => cm.ChannelId == channelId && cm.UserId == user.Id);

            if (!isMember) return StatusCode(403, "Ви не є учасником цього чату.");

            // Дістаємо повідомлення, сортуємо за часом і підтягуємо імена відправників
            var messages = await _context.Messages
                .Where(m => m.ChannelId == channelId)
                .Include(m => m.User)
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.Id,
                    m.Content,
                    m.Timestamp,
                    SenderName = m.User.UserName,
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}