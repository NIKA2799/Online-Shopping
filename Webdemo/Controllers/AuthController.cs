using Interface.Model;
using Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid registration data.");

            var result = await _authService.RegisterAsync(model);
            if (result.StartsWith("User registered"))
                return Ok(new { message = result });

            return BadRequest(new { error = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login data.");

            var token = await _authService.LoginAsync(model);
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { error = "Invalid credentials" });

            return Ok(new { token });
        }

        // Optionally protected endpoint to validate token or return profile
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userName = User.Identity?.Name;
            var roles = User.Claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            return Ok(new { user = userName, roles });
        }
    }
}
