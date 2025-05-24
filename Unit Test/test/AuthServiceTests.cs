using Castle.Core.Configuration;
using Dto;
using Interface.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.test
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _configMock; // Ensure this uses Microsoft.Extensions.Configuration
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var keySection = new Mock<IConfigurationSection>();
            keySection.Setup(x => x.Value).Returns("THIS_IS_A_FAKE_JWT_SECRET_KEY_FOR_TESTING_PURPOSE");

            var issuerSection = new Mock<IConfigurationSection>();
            issuerSection.Setup(x => x.Value).Returns("testissuer");

            var audienceSection = new Mock<IConfigurationSection>();
            audienceSection.Setup(x => x.Value).Returns("testaudience");

            _configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            _configMock.Setup(x => x.GetSection("Jwt:Key")).Returns(keySection.Object);
            _configMock.Setup(x => x.GetSection("Jwt:Issuer")).Returns(issuerSection.Object);
            _configMock.Setup(x => x.GetSection("Jwt:Audience")).Returns(audienceSection.Object);

            _authService = new AuthService(_userManagerMock.Object, _roleManagerMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test@123",
                Role = "Customer"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.Equal("User registered successfully.", result);
        }
    }
}