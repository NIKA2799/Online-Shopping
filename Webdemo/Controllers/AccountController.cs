using System.Threading.Tasks;
using Interface;
using Interface.Command;
using Interface.Model;
using Microsoft.AspNetCore.Mvc;

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
            => _accountService = accountService;

        /// <summary>
        /// Registers a new user.  Returns a JSON payload containing the confirmation link.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var (success, error, result) = await _accountService.RegisterAsync(
                model,
                // urlAction: (action, controller, values) => full URL
                (action, controller, values) =>
                    Url.Action(action, controller, values, Request.Scheme)!
            );

            if (!success)
                return BadRequest(new { Message = error });

            // returns: { message, confirmationLink }
            return Ok(result);
        }

        /// <summary>
        /// GET /api/account/confirm-email?userId=...&token=...
        /// Confirms the user's email.
        /// </summary>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string token
        )
        {
            var (success, error) = await _accountService.ConfirmEmailAsync(userId, token);
            if (!success)
                return BadRequest(new { Message = error });

            return Ok(new { Message = "Email confirmed successfully." });
        }

        /// <summary>
        /// Logs in and returns a JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (success, error, result) = await _accountService.LoginAsync(model);
            if (!success)
                return Unauthorized(new { Message = error });

            // returns: { Token = "...", Expires = ... }
            return Ok(result);
        }

        /// <summary>
        /// Generates a password‐reset link and returns it in JSON.
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            var (success, error, result) = await _accountService.ForgotPasswordAsync(
                email,
                (action, controller, values) =>
                    Url.Action(action, controller, values, Request.Scheme)!
            );

            if (!success)
                return BadRequest(new { Message = error });

            // returns: { ResetLink = "https://..." }
            return Ok(result);
        }

        /// <summary>
        /// Resets a password.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var (success, error) = await _accountService.ResetPasswordAsync(
                model.UserId,
                model.Token,
                model.NewPassword
            );

            if (!success)
                return BadRequest(new { Message = error });

            return Ok(new { Message = "Password has been reset successfully." });
        }
    }
}
