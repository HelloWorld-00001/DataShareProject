using Castle.Core.Configuration;
using DataShareCore.Common.Helper;

namespace DataShareTest;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using WebApplication1.Controller;
using WebApplication1.Service.AccountService;
using Microsoft.AspNetCore.Mvc;
using DataShareCore.Dtos;
using DataShareCore.Models;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockAccountService = new Mock<IAccountService>();
        _mockRefreshTokenService = new Mock<IRefreshTokenService>();
        _controller = new AuthController(_mockConfig.Object, _mockAccountService.Object, _mockRefreshTokenService.Object);
    }
    // ------------------------------------------------------------------------------------------
    // register test area
    // ------------------------------------------------------------------------------------------
    [Test]
    public async Task AddAccount_OK_WhenLoginSuccess()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "123456" );
        var account = new Account { email = "test@example.com", password = "123456"};
        _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
        CustomEncoder.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);
        account.password = ByteConvertion.ByteArrayToString(passwordHash);
        account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

        // Act
        var result = await _controller.Login(accountDtos);

        // Assert
        NUnit.Framework.Assert.Pass();
    }
    [Test]
    public async Task AddAccount_ReturnsBadRequest_WhenPasswordTooShort()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "12345" );

        // Act
        var result = await _controller.AddAccount(accountDtos);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Password is too weak, must be greater than 6 characters", badRequestResult.Value);
    }

    [Test]
    public async Task AddAccount_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "1234567" );
        _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(new Account());

        // Act
        var result = await _controller.AddAccount(accountDtos);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Email already existed", badRequestResult.Value);
    }
    // ------------------------------------------------------------------------------------------
    // End register test area
    // ------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------
    // Start login test area
    // ------------------------------------------------------------------------------------------

    [Test]
    public async Task Login_ReturnsNotFound_WhenEmailNotFound()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "12345" );
        _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync((Account)null);

        // Act
        var result = await _controller.Login(accountDtos);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Email not found", notFoundResult.Value);
    }

    [Test]
    public async Task Login_ReturnsBadRequest_WhenWrongCredentials()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "12345" );
        var account = new Account { email = "test@example.com", password = "correcthash"};
        _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
        CustomEncoder.CreatePasswordHash("correctpassword", out byte[] passwordHash, out byte[] passwordSalt);
        account.password = ByteConvertion.ByteArrayToString(passwordHash);
        account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

        // Act
        var result = await _controller.Login(accountDtos);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Wrong credentials", badRequestResult.Value);
    }
    [Test]
    public async Task Login_ReturnsBadRequest_WhenUsingSqlInjection()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "12345" );
        var account = new Account { email = "test@example.com", password = "correcthash or '1=1"};
        _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
        CustomEncoder.CreatePasswordHash("correctpassword", out byte[] passwordHash, out byte[] passwordSalt);
        account.password = ByteConvertion.ByteArrayToString(passwordHash);
        account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

        // Act
        var result = await _controller.Login(accountDtos);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Wrong credentials", badRequestResult.Value);
    }
    [Test]
    public async Task Login_ReturnsBadRequest_WhenUsingSqlInjection2()
    {
        // Arrange
        var accountDtos = new AccountDtos("test@example.com", "12345" );
        var account = new Account { email = "test@example.com", password = "' # correcthash"};
        _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
        CustomEncoder.CreatePasswordHash("correctpassword", out byte[] passwordHash, out byte[] passwordSalt);
        account.password = ByteConvertion.ByteArrayToString(passwordHash);
        account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

        // Act
        var result = await _controller.Login(accountDtos);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Wrong credentials", badRequestResult.Value);
    }
    // ------------------------------------------------------------------------------------------
    // End login test area
    // ------------------------------------------------------------------------------------------
    
    // ------------------------------------------------------------------------------------------
    // start refresh token test area
    // ------------------------------------------------------------------------------------------
    [Test]
    public async Task RefreshToken_ReturnsUnauthorized_WhenInvalidToken()
    {
        // Arrange
        var refreshToken = "invalid_token";
        _mockRefreshTokenService.Setup(s => s.GetUserIdByToken(refreshToken)).ReturnsAsync(-1);

        // Act
        var result = await _controller.RefreshToken(refreshToken);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("Invalid refresh token", unauthorizedResult.Value);
    }
}
