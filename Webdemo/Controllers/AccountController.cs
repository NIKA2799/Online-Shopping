using Dto;
using Interface;
using Interface.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using Webdemo.Exstnsion;

namespace Webdemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, result) = await _accountService.RegisterAsync(
                model,
                (action, controller, values) => Url.Action(action, controller, values, Request.Scheme) ?? string.Empty
            );

            if (!success)
                return BadRequest(error);

            return Ok(result);
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var (success, error) = await _accountService.ConfirmEmailAsync(userId, token);
            if (success)
                return Ok("Email confirmed successfully.");
            return BadRequest(error);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (success, error, result) = await _accountService.LoginAsync(model);
            if (!success)
                return Unauthorized(error);
            return Ok(result);
        }
    }
}