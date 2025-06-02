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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly WebDemoDbContext _context;
        private readonly IEmailConfiguration _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ITokenService tokenService, WebDemoDbContext context, IEmailConfiguration emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest("User with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelState);
            }

            // ➕ მომხმარებლისთვის როლის მინიჭება
            await _userManager.AddToRoleAsync(user, "user");

            // ➕ Customer-ის შექმნა
            var customer = new Customer
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                Password = model.Password, // დაიჰეშება ავტომატურად
                ApplicationUserId = user.Id
            };

            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            // ➕ Email-ის დადასტურების ლინკის გენერაცია
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                                              new { userId = user.Id, token = token }, Request.Scheme);

            return Ok(new
            {
                message = "Registration successful. Please confirm your email.",
                confirmationLink
            });
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest("Invalid email confirmation request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // ✅ Fix: Correctly pass all required parameters to SendEmailAsync
#pragma warning disable CS8604 // Possible null reference argument.
                await _emailSender.SendEmailAsync(user.Email, "Email Confirmed",
                    $"Hello {user.UserName}, your email has been successfully confirmed. 🎉");
#pragma warning restore CS8604 // Possible null reference argument.

                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Email confirmation failed.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            if (!user.EmailConfirmed)
                return Unauthorized("Email not confirmed.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(new { token });
        }
    }
}