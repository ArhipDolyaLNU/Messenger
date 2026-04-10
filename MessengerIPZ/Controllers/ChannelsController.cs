using Messenger.Data;
using MessengerIPZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

    }
}

