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
    public class ChannelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public ChannelsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class CreateChannelDto
        {
            public string Name { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelDto model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var channel = new Channel()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CreatedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channel);

            var member = new ChannelMember()
            {
                ChannelId = channel.Id,
                UserId = user.Id,
                JoinedAt = DateTime.UtcNow
            };

            _context.ChannelMembers.Add(member);

            await _context.SaveChangesAsync();

            return Ok(new { channel.Id, channel.Name });

        }

        // Отримання списку всіх каналів
        [HttpGet]
        public async Task<IActionResult> GetChannels()
        {
            var channels = await _context.Channels
                .Select(c => new {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.IsPrivate,
                    MemberCount = c.Members.Count
                })
                .ToListAsync();

            return Ok(channels);
        }

        // Приєднання до існуючого каналу
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinChannel(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Шукаємо канал
            var channel = await _context.Channels.FindAsync(id);
            if (channel == null) return NotFound("Канал не знайдено.");

            // Перевіряємо, чи юзер вже є учасником
            var isAlreadyMember = await _context.ChannelMembers
                .AnyAsync(cm => cm.ChannelId == id && cm.UserId == user.Id);

            if (isAlreadyMember) return BadRequest("Ви вже є учасником цього чату.");

            // Додаємо юзера в чат
            var member = new ChannelMember()
            {
                ChannelId = id,
                UserId = user.Id,
                JoinedAt = DateTime.UtcNow
            };

            _context.ChannelMembers.Add(member);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Ви успішно приєдналися до каналу {channel.Name}" });
        }

    }
}

