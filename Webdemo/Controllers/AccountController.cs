using Interface;
using Interface.Command;
using Interface.Model;
using Microsoft.AspNetCore.Http;
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
        /// Registers a new user.  Returns { message, confirmationLink } on success.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // model is pre‑validated by FluentValidation / [ApiController]
            var (success, error, result) = await _accountService.RegisterAsync(
                model,
                (action, controller, values) =>
                    Url.Action(action, controller, values, Request.Scheme)!
            );

            if (!success)
                return BadRequest(new { message = error });

            return Ok(result);
        }

        /// <summary>
        /// Callback link: /api/account/confirm-email?userId=...&token=...
        /// </summary>
        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string token
        )
        {
            var (success, error) = await _accountService.ConfirmEmailAsync(userId, token);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Email confirmed successfully." });
        }

        /// <summary>
        /// Logs in and returns { token } on success.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (success, error, result) = await _accountService.LoginAsync(model);
            if (!success)
                return Unauthorized(new { message = error });

            return Ok(result);
        }

        /// <summary>
        /// Generates a password reset link and returns { resetLink }.
        /// </summary>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var (success, error, result) = await _accountService.ForgotPasswordAsync(
                model.Email,
                (action, controller, values) =>
                    Url.Action(action, controller, values, Request.Scheme)!
            );

            if (!success)
                return BadRequest(new { message = error });

            return Ok(result);
        }

        /// <summary>
        /// Resets password.  Expects { userId, token, newPassword } in the body.
        /// </summary>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var (success, error) = await _accountService.ResetPasswordAsync(
                model.UserId,
                model.Token,
                model.NewPassword
            );

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
