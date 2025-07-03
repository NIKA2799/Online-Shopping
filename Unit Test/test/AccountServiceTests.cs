using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Interface;
using Microsoft.AspNetCore.Identity;
using Moq;
using Service.CommandService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.test
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailConfiguration> _emailSenderMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            var store =
    new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();
            _userServiceMock = new Mock<IUserService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailSenderMock = new Mock<IEmailConfiguration>();

            _accountService = new AccountService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object,
                _userServiceMock.Object,
                _emailSenderMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenUserIsCreated()
        {
            // Arrange
            var model = new RegisterModel { Email = "test@example.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "user")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("token");

            // Act
            var result = await _accountService.RegisterAsync(model, (a, b, c) => "fakeUrl");

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ReturnsTrue_WhenConfirmationSucceeds()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@example.com", UserName = "test" };
            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, "validToken")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountService.ConfirmEmailAsync("1", "validToken");

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com", EmailConfirmed = true };
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "Password123", false)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });
            _tokenServiceMock.Setup(x => x.GenerateToken(user, It.IsAny<IList<string>>())).Returns("fake-jwt-token");

            var model = new LoginModel { Email = user.Email, Password = "Password123" };

            // Act
            var result = await _accountService.LoginAsync(model);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Result);
        }
    }
}