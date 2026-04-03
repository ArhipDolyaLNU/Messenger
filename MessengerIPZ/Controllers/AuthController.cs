using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MessengerIPZ.Models;

namespace MessengerIPZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AuthController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new User
            {
                UserName = username,
                IsOnline = true,
                LastSeen = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return Unauthorized();

            var valid = await _userManager.CheckPasswordAsync(user, password);

            if (!valid)
                return Unauthorized();

            return Ok("Logged in");
        }
    }
}
