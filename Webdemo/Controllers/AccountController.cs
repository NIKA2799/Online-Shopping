using Dto;
using Interface;
using Interface.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using Webdemo.Exstnsion;

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // urlAction: ფუნქცია ბმულის ასაგებად
            string urlAction(string action, string controller, object routeValues)
            {
                return Url.Action(action, controller, routeValues, Request.Scheme);
            }

            var (success, error, result) = await _accountService.RegisterAsync(model, urlAction);

            if (!success)
                return BadRequest(new { error });

            return Ok(result);
        }

        // GET: /Account/ConfirmEmail?userId=...&token=...
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var (success, error) = await _accountService.ConfirmEmailAsync(userId, token);
            if (!success)
                return BadRequest(new { error });

            // Success page ან რიდაირექცია შეგიძლია დაამატო
            return Ok(new { message = "Email confirmed successfully." });
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, result) = await _accountService.LoginAsync(model);

            if (!success)
                return Unauthorized(new { error });

            return Ok(result);
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string urlAction(string action, string controller, object routeValues)
            {
                return Url.Action(action, controller, routeValues, Request.Scheme);
            }

            var (success, error) = await _accountService.ForgotPasswordAsync(model.Email, urlAction);

            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Password reset link sent to your email." });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error) = await _accountService.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);

            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Password reset successful." });
        }
    }
}