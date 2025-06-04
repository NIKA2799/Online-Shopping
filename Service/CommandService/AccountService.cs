using Dto;
using Interface.Model;
using Interface;
using Microsoft.AspNetCore.Identity;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface.IRepositories;
using Interface.Command;

namespace Service.CommandService
{
    public class AccountService: IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ICustomerService _customerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailConfiguration _emailSender;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            WebDemoDbContext context,
            IUnitOfWork unitOfWork,
           ICustomerService customerService,

        IEmailConfiguration emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string? Error, object? Result)> RegisterAsync(RegisterModel model, Func<string, string, object, string> urlAction)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return (false, "User with this email already exists.", null);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (false, string.Join("; ", errors), null);
            }

            await _userManager.AddToRoleAsync(user, "user");

            // ⬇️ აქ ვიყენებთ შენს UserService-ს
            await _customerService.CreateCustomerAsync(model, user.Id);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = urlAction("ConfirmEmail", "Account", new { userId = user.Id, token });

            return (true, null, new
            {
                message = "Registration successful. Please confirm your email.",
                confirmationLink
            });
        }

        public async Task<(bool Success, string Error)> ConfirmEmailAsync(string userId, string token)
        {
            if (userId == null || token == null)
                return (false, "Invalid email confirmation request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(user.Email)) // Ensure user.Email is not null
                {
                    await _emailSender.SendEmailAsync(user.Email, "Email Confirmed",
                        $"Hello {user.UserName}, your email has been successfully confirmed. 🎉");
                }
                return (true, string.Empty); // Replace null with string.Empty to match the non-nullable 'Error' type
            }
            else
            {
                return (false, "Email confirmation failed.");
            }
        }

        public async Task<(bool Success, string? Error, object? Result)> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "Invalid credentials", null);

            if (!user.EmailConfirmed)
                return (false, "Email not confirmed.", null);

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return (false, "Invalid credentials", null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return (true, null, new { token });
        }
    }
}
