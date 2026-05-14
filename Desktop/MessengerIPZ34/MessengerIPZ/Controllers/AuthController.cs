using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MessengerIPZ.Models;
using Microsoft.AspNetCore.Authorization;

namespace MessengerIPZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public class DataDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DataDto model)
        {
            var user = new User
            {
                UserName = model.Username,
                IsOnline = true,
                LastSeen = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DataDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid login");
            }

            return Ok("Logged in");
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(User.Identity.Name);
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            // Повертаємо всіх користувачів, крім поточного
            var users = _userManager.Users
                .Where(u => u.Id != currentUser.Id)
                .Select(u => new {
                    u.Id,
                    u.UserName,
                    u.IsOnline,
                    u.LastSeen
                })
                .ToList();

            return Ok(users);
        }
    }
}