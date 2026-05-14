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

        // Отримання списку доступних каналів
        [HttpGet]
        public async Task<IActionResult> GetChannels()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var channels = await _context.Channels
                // Показуємо тільки публічні канали, АБО приватні, де юзер є учасником
                .Where(c => !c.IsPrivate || c.Members.Any(m => m.UserId == user.Id))
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

        public class CreatePrivateChatDto
        {
            public List<string> TargetUserIds { get; set; }
        }

        [HttpPost("private")]
        public async Task<IActionResult> GetOrCreatePrivateChat([FromBody] CreatePrivateChatDto request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            if (request.TargetUserIds == null || !request.TargetUserIds.Any())
                return BadRequest("Вкажіть хоча б одного користувача.");

            // Формуємо повний список учасників (поточний юзер + всі обрані)
            var allParticipantIds = request.TargetUserIds.Append(currentUser.Id).Distinct().ToList();

            // 1. Шукаємо, чи вже є приватний чат з ТОЧНО таким самим складом учасників
            var candidateChannels = await _context.Channels
                .Include(c => c.Members)
                .Where(c => c.IsPrivate && c.Members.Count == allParticipantIds.Count)
                .ToListAsync();

            var existingChannel = candidateChannels.FirstOrDefault(c =>
                c.Members.All(m => allParticipantIds.Contains(m.UserId)));

            if (existingChannel != null)
            {
                return Ok(new { existingChannel.Id, existingChannel.Name, existingChannel.IsPrivate });
            }

            // 2. Якщо немає - створюємо новий
            var channel = new Channel()
            {
                Id = Guid.NewGuid(),
                Name = "Private Chat",
                IsPrivate = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channel);

            foreach (var userId in allParticipantIds)
            {
                _context.ChannelMembers.Add(new ChannelMember
                {
                    ChannelId = channel.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { channel.Id, channel.Name, channel.IsPrivate });
        }

        public class AddMemberDto
        {
            public string UserId { get; set; }
        }

        [HttpPost("{id}/add-member")]
        public async Task<IActionResult> AddMemberToPrivateChat(Guid id, [FromBody] AddMemberDto model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var channel = await _context.Channels
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (channel == null) return NotFound("Канал не знайдено.");

            if (!channel.Members.Any(m => m.UserId == currentUser.Id))
                return StatusCode(403, "Ви не можете додавати людей у цей чат.");

            if (channel.Members.Any(m => m.UserId == model.UserId))
                return BadRequest("Користувач вже є учасником чату.");

            var newMember = new ChannelMember()
            {
                ChannelId = id,
                UserId = model.UserId,
                JoinedAt = DateTime.UtcNow
            };

            _context.ChannelMembers.Add(newMember);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Користувача успішно додано до чату." });
        }

    }
}

