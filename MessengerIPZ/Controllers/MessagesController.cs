using Messenger.Data;
using MessengerIPZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR; 
using MessengerIPZ.Hubs; 

namespace MessengerIPZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _hubContext; 

        public MessagesController(ApplicationDbContext context, UserManager<User> userManager, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
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

            var isMember = await _context.ChannelMembers
                .AnyAsync(cm => cm.ChannelId == model.ChannelId && cm.UserId == user.Id);

            if (!isMember) return StatusCode(403, "Ви не є учасником цього чату і не можете писати сюди.");

            var message = new Message
            {
                ChannelId = model.ChannelId,
                UserId = user.Id,
                Content = model.Content,
                Timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                Id = message.Id,
                Content = message.Content,
                Timestamp = message.Timestamp,
                Sender = user.UserName
            };

            await _hubContext.Clients.Group(model.ChannelId.ToString())
                .SendAsync("ReceiveMessage", responseData.Sender, responseData.Content);

            return Ok(responseData);
        }

        [HttpGet("{channelId}")]
        public async Task<IActionResult> GetMessages(Guid channelId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isMember = await _context.ChannelMembers
                .AnyAsync(cm => cm.ChannelId == channelId && cm.UserId == user.Id);

            if (!isMember) return StatusCode(403, "Ви не є учасником цього чату.");

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