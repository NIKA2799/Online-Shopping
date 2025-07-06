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
        // ASP.NET Core Identity services for user management and sign-in
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        // Custom services for token generation, data persistence, user profile, and email sending
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountService> _logger;

        /// <summary>
        /// Constructor with dependency injection. Validates all injected services.
        /// </summary>
        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IUserService userService,
            IEmailSender emailSender,
            ILogger<AccountService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registers a new user, assigns the default role, creates related customer data, and sends a confirmation email.
        /// </summary>
        /// <param name="model">Contains registration data (email, password, etc.).</param>
        /// <param name="urlAction">Function to generate URLs for email confirmation links.</param>
        public async Task<(bool Success, string? Error, object? Result)> RegisterAsync(
            RegisterModel model,
            Func<string, string, object, string> urlAction)
        {
            // Validate input
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (urlAction == null) throw new ArgumentNullException(nameof(urlAction));

            // Check if the email is already in use
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser is not null)
            {
                _logger.LogWarning("Registration attempt failed: email already exists ({Email})", model.Email);
                return (false, "A user with this email already exists.", null);
            }

            // Create new identity user instance
            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false // Will be confirmed via email link
            };

            // Attempt to create the user with provided password
            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!createResult.Succeeded)
            {
                // Aggregate errors to a single string for response
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {Email}: {Errors}", model.Email, errors);
                return (false, errors, null);
            }

            // Assign default "user" role
            await _userManager.AddToRoleAsync(newUser, "user");

            // Create a corresponding customer record in domain data
            await _userService.CreateCustomerAsync(model, newUser.Id);

            // Generate email confirmation token and link
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationLink = urlAction(
                nameof(ConfirmEmailAsync),      // target method name
                "Account",                     // controller name
                new { userId = newUser.Id, token } // route values
            );

            // Send the confirmation email with embedded link
            await _emailSender.SendEmailAsync(
                newUser.Email,
                "Please confirm your email",
                $"Hello {newUser.UserName},<br/>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>."
            );
            _logger.LogInformation("Sent email confirmation to {Email}", newUser.Email);

            // Return success with info for client
            return (true, null, new
            {
                Message = "Registration successful. Please check your email to confirm your account.",
                ConfirmationLink = confirmationLink
            });
        }

        /// <summary>
        /// Confirms the user's email based on a token generated at registration.
        /// </summary>
        public async Task<(bool Success, string Error)> ConfirmEmailAsync(string userId, string token)
        {
            // Ensure parameters are valid
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return (false, "Invalid email confirmation request.");

            // Fetch the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            // Attempt to confirm the email with the provided token
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed for user {UserId}: {Errors}", userId, errors);
                return (false, "Email confirmation failed.");
            }

            // Notify user via email that confirmation succeeded
            await _emailSender.SendEmailAsync(
                user.Email,
                "Email Confirmed",
                $"Hello {user.UserName},<br/>Your email has been successfully confirmed. 🎉"
            );
            _logger.LogInformation("Email confirmed for user {UserId}", userId);

            return (true, string.Empty);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        public async Task<(bool Success, string? Error, object? Result)> LoginAsync(LoginModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Find user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "Invalid credentials", null);

            // Ensure email has been confirmed
            if (!user.EmailConfirmed)
                return (false, "Email not confirmed.", null);

            // Validate password
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!signInResult.Succeeded)
            {
                _logger.LogWarning("Failed login attempt for {Email}", model.Email);
                return (false, "Invalid credentials", null);
            }

            // Retrieve roles and generate JWT
            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.GenerateToken(user, roles);

            _logger.LogInformation("User {Email} logged in successfully", model.Email);
            return (true, null, new { Token = jwtToken });
        }
        /// <summary>
        /// Sends a password reset email to the user.
        /// </summary>
        public async Task<(bool Success, string? Error)> ForgotPasswordAsync(string email, Func<string, string, object, string> urlAction)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = urlAction(nameof(ResetPasswordAsync), "Account", new { userId = user.Id, token });

            await _emailSender.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                $"To reset your password, <a href='{resetLink}'>click here</a>."
            );
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                return (false, "Invalid request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for user {UserId}: {Errors}", userId, errors);
                return (false, errors);
            }

            _logger.LogInformation("Password reset succeeded for user {UserId}", userId);
            return (true, null);
        }
    }
}