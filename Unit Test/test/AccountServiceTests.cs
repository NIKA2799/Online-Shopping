// AccountServiceTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Resource;
using Dto; // ← adjust if your models live elsewhere
using Interface;
using Interface.Command; // IAccountService
using Interface.IRepositories;
using Interface.Model; // RegisterModel, LoginModel, etc.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Service.CommandService;
using Xunit;

public class AccountServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<ILogger<AccountService>> _logger;

    private readonly AccountService _svc;

    public AccountServiceTests()
    {
        _userManager = CreateUserManagerMock();
        _signInManager = CreateSignInManagerMock(_userManager.Object);
        _tokenService = new Mock<ITokenService>();
        _userService = new Mock<IUserService>();
        _uow = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<AccountService>>();

        _svc = new AccountService(
            _userManager.Object,
            _signInManager.Object,
            _tokenService.Object,
            _uow.Object,
            _userService.Object,
            _logger.Object
        );
    }

    // -------------------- RegisterAsync --------------------

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ReturnsFalse()
    {
        var model = new RegisterModel { Email = "a@a.com", Password = "P@ssw0rd!" };

        _userManager.Setup(m => m.FindByEmailAsync(model.Email))
                    .ReturnsAsync(new ApplicationUser { Email = model.Email });

        var res = await _svc.RegisterAsync(model, DummyUrl);

        Assert.False(res.Success);
        Assert.Equal("A user with that email already exists.", res.Error);
        Assert.Null(res.Result);
    }

    [Fact]
    public async Task RegisterAsync_Success_CreatesUser_AddsRole_CreatesCustomer_ReturnsLink()
    {
        var model = new RegisterModel
        {
            Email = "new@user.com",
            Password = "P@ssw0rd!",
            Name = "New User",
            PhoneNumber = "+995551234567",
            ShippingAddress = "Tbilisi",
            BillingAddress = "Tbilisi"
        };

        _userManager.Setup(m => m.FindByEmailAsync(model.Email))
                    .ReturnsAsync((ApplicationUser)null!);

        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                    .ReturnsAsync(IdentityResult.Success);

        _userManager.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Customer"))
                    .ReturnsAsync(false);

        _userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"))
                    .ReturnsAsync(IdentityResult.Success);

        _userManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                    .ReturnsAsync("confirm-token");

        // <<< მნიშვნელოვანი ნაწილი
        _userService
            .Setup(s => s.CreateCustomerAsync(It.IsAny<RegisterModel>(), It.IsAny<string>()))
            .ReturnsAsync(new Dto.User { Id = 1, Name = model.Name, Email = model.Email, DateCreated = DateTime.UtcNow });

        var res = await _svc.RegisterAsync(model, (a, c, v) => "https://test/confirm");

        Assert.True(res.Success);
        Assert.Null(res.Error);
        Assert.NotNull(res.Result);

        _userManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
        _userManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"), Times.Once);
        _userService.Verify(s => s.CreateCustomerAsync(It.IsAny<RegisterModel>(), It.IsAny<string>()), Times.Once);
    }


    // -------------------- ConfirmEmailAsync --------------------

    [Theory]
    [InlineData(null, "tok")]
    [InlineData("", "tok")]
    [InlineData("uid", "")]
    public async Task ConfirmEmailAsync_InvalidInput_ReturnsFalse(string userId, string token)
    {
        var (ok, err) = await _svc.ConfirmEmailAsync(userId, token);
        Assert.False(ok);
        Assert.Equal("Invalid confirmation request.", err);
    }

    [Fact]
    public async Task ConfirmEmailAsync_UserNotFound_ReturnsFalse()
    {
        _userManager.Setup(m => m.FindByIdAsync("uid")).ReturnsAsync((ApplicationUser)null!);

        var (ok, err) = await _svc.ConfirmEmailAsync("uid", "tok");

        Assert.False(ok);
        Assert.Equal("User not found.", err);
    }

    [Fact]
    public async Task ConfirmEmailAsync_Failed_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = "u1", Email = "a@a.com" };
        _userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManager.Setup(m => m.ConfirmEmailAsync(user, "tok"))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "bad tok" }));

        var (ok, err) = await _svc.ConfirmEmailAsync(user.Id, "tok");

        Assert.False(ok);
        Assert.Contains("Email confirmation failed", err);
    }

    [Fact]
    public async Task ConfirmEmailAsync_Success_ReturnsTrue()
    {
        var user = new ApplicationUser { Id = "u1", Email = "a@a.com" };
        _userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManager.Setup(m => m.ConfirmEmailAsync(user, "tok"))
                    .ReturnsAsync(IdentityResult.Success);

        var (ok, err) = await _svc.ConfirmEmailAsync(user.Id, "tok");

        Assert.True(ok);
        Assert.Equal(string.Empty, err);
    }

    // -------------------- LoginAsync --------------------

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsFalse()
    {
        var model = new LoginModel { Email = "x@x.com", Password = "pass" };
        _userManager.Setup(m => m.FindByEmailAsync(model.Email))
                    .ReturnsAsync((ApplicationUser)null!);

        var res = await _svc.LoginAsync(model);

        Assert.False(res.Success);
        Assert.Equal("Invalid credentials.", res.Error);
    }

    [Fact]
    public async Task LoginAsync_EmailNotConfirmed_ReturnsFalse()
    {
        var user = new ApplicationUser { Email = "x@x.com", EmailConfirmed = false };
        var model = new LoginModel { Email = "x@x.com", Password = "pass" };

        _userManager.Setup(m => m.FindByEmailAsync(model.Email))
                    .ReturnsAsync(user);

        var res = await _svc.LoginAsync(model);

        Assert.False(res.Success);
        Assert.Equal("Email not confirmed.", res.Error);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFalse()
    {
        var user = new ApplicationUser { Email = "x@x.com", EmailConfirmed = true };
        var model = new LoginModel { Email = "x@x.com", Password = "pass" };

        _userManager.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _signInManager.Setup(s => s.CheckPasswordSignInAsync(user, model.Password, false))
                      .ReturnsAsync(SignInResult.Failed);

        var res = await _svc.LoginAsync(model);

        Assert.False(res.Success);
        Assert.Equal("Invalid credentials.", res.Error);
    }

    [Fact]
    public async Task LoginAsync_Success_ReturnsToken()
    {
        var user = new ApplicationUser { Id = "u1", Email = "x@x.com", EmailConfirmed = true };
        var model = new LoginModel { Email = "x@x.com", Password = "pass" };

        _userManager.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _signInManager.Setup(s => s.CheckPasswordSignInAsync(user, model.Password, false))
                      .ReturnsAsync(SignInResult.Success);
        _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });

        _tokenService.Setup(t => t.GenerateToken(user, It.IsAny<IList<string>>()))
                     .Returns("JWT_TOKEN");

        var res = await _svc.LoginAsync(model);

        Assert.True(res.Success);
        Assert.NotNull(res.Result);
        Assert.Null(res.Error);
        Assert.Equal("JWT_TOKEN", res.Result!.GetType().GetProperty("Token")!.GetValue(res.Result));
    }

    // -------------------- ForgotPasswordAsync --------------------

    [Fact]
    public async Task ForgotPasswordAsync_UserNotFound_ReturnsFalse()
    {
        _userManager.Setup(m => m.FindByEmailAsync("x@x.com"))
                    .ReturnsAsync((ApplicationUser)null!);

        var res = await _svc.ForgotPasswordAsync("x@x.com", DummyUrl);

        Assert.False(res.Success);
        Assert.Equal("User not found.", res.Error);
    }

    [Fact]
    public async Task ForgotPasswordAsync_Success_ReturnsLink()
    {
        var user = new ApplicationUser { Id = "u1", Email = "x@x.com" };
        _userManager.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user))
                    .ReturnsAsync("reset-token");

        var res = await _svc.ForgotPasswordAsync(user.Email, DummyUrl);

        Assert.True(res.Success);
        Assert.NotNull(res.Result);
        Assert.Contains("http", res.Result!.GetType().GetProperty("ResetLink")!.GetValue(res.Result)!.ToString());
    }

    // -------------------- ResetPasswordAsync --------------------

    [Theory]
    [InlineData("", "t", "p")]
    [InlineData("u", "", "p")]
    [InlineData("u", "t", "")]
    public async Task ResetPasswordAsync_InvalidRequest_ReturnsFalse(string uid, string tok, string pwd)
    {
        var (ok, err) = await _svc.ResetPasswordAsync(uid, tok, pwd);
        Assert.False(ok);
        Assert.Equal("Invalid password reset request.", err);
    }

    [Fact]
    public async Task ResetPasswordAsync_UserNotFound_ReturnsFalse()
    {
        _userManager.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser)null!);

        var (ok, err) = await _svc.ResetPasswordAsync("u1", "tok", "NewP@ss1");
        Assert.False(ok);
        Assert.Equal("User not found.", err);
    }

    [Fact]
    public async Task ResetPasswordAsync_Failed_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = "u1", Email = "x@x.com" };
        _userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManager.Setup(m => m.ResetPasswordAsync(user, "tok", "NewP@ss1"))
                    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "bad token" }));

        var (ok, err) = await _svc.ResetPasswordAsync(user.Id, "tok", "NewP@ss1");

        Assert.False(ok);
        Assert.Contains("bad token", err);
    }

    [Fact]
    public async Task ResetPasswordAsync_Success_ReturnsTrue()
    {
        var user = new ApplicationUser { Id = "u1", Email = "x@x.com" };
        _userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManager.Setup(m => m.ResetPasswordAsync(user, "tok", "NewP@ss1"))
                    .ReturnsAsync(IdentityResult.Success);

        var (ok, err) = await _svc.ResetPasswordAsync(user.Id, "tok", "NewP@ss1");

        Assert.True(ok);
        Assert.Null(err);
    }

    // --------------- helpers ---------------

    private static Func<string, string, object, string> DummyUrl =>
        (action, controller, values) => $"https://test/{controller}/{action}?ok=1";

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> um)
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        return new Mock<SignInManager<ApplicationUser>>(
            um,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null
        );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }


    // If your IUserService returns a specific domain type, define a dummy minimal class for test
    // or reference your actual domain model. Here is a placeholder:
    private class User { } // ← remove if you already have a domain User/Customer type in a referenced project
}
