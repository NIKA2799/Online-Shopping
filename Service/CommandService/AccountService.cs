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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Service.CommandService
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<AccountService> logger
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string? Error, object? Result)> RegisterAsync(
            RegisterModel model,
            Func<string, string, object, string> urlAction
        )
        {
            if (model is null) throw new ArgumentNullException(nameof(model));
            if (urlAction is null) throw new ArgumentNullException(nameof(urlAction));

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                _logger.LogWarning("Registration failed: {Email} already in use", model.Email);
                return (false, "A user with that email already exists.", null);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {Email}: {Errors}", model.Email, errors);
                return (false, errors, null);
            }

            const string DefaultRole = "Customer";
            if (!await _userManager.IsInRoleAsync(user, DefaultRole))
                await _userManager.AddToRoleAsync(user, DefaultRole);

            // Create corresponding domain‐customer record
            await _userService.CreateCustomerAsync(model, user.Id);

            // Generate email confirmation link but do NOT send email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = urlAction(
                nameof(ConfirmEmailAsync),
                "Account",
                new { userId = user.Id, token }
            );

            _logger.LogInformation("Registration succeeded for {Email}", model.Email);
            return (true, null, new
            {
                Message = "Registration successful. Please confirm via the returned link.",
                ConfirmationLink = link
            });
        }

        public async Task<(bool Success, string Error)> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return (false, "Invalid confirmation request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed for {UserId}: {Errors}", userId, errors);
                return (false, "Email confirmation failed: " + errors);
            }

            _logger.LogInformation("Email confirmed for {UserId}", userId);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string? Error, object? Result)> LoginAsync(LoginModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "Invalid credentials.", null);

            if (!user.EmailConfirmed)
                return (false, "Email not confirmed.", null);

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!signIn.Succeeded)
                return (false, "Invalid credentials.", null);

            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.GenerateToken(user, roles);

            _logger.LogInformation("User {Email} logged in", model.Email);
            return (true, null, new { Token = jwtToken });
        }

        public async Task<(bool Success, string? Error, object? Result)> ForgotPasswordAsync(
            string email,
            Func<string, string, object, string> urlAction
        )
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found.", null);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = urlAction(
                nameof(ResetPasswordAsync),
                "Account",
                new { userId = user.Id, token }
            );

            _logger.LogInformation("Generated password reset link for {Email}", email);
            return (true, null, new { ResetLink = link });
        }

        public async Task<(bool Success, string? Error)> ResetPasswordAsync(
            string userId,
            string token,
            string newPassword
        )
        {
            if (string.IsNullOrWhiteSpace(userId)
             || string.IsNullOrWhiteSpace(token)
             || string.IsNullOrWhiteSpace(newPassword))
                return (false, "Invalid password reset request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for {UserId}: {Errors}", userId, errors);
                return (false, errors);
            }

            _logger.LogInformation("Password reset succeeded for {UserId}", userId);
            return (true, null);
        }
    }
}