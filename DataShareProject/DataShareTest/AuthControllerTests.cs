using Castle.Core.Configuration;
using DataShareCore.Common.Helper;
using Moq;
using Microsoft.Extensions.Configuration;
using WebApplication1.Controller;
using WebApplication1.Service.AccountService;
using Microsoft.AspNetCore.Mvc;
using DataShareCore.Dtos;
using DataShareCore.Models;
using DataShareCore.Response;
using Microsoft.AspNetCore.Http;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace DataShareTest
{
    public class AuthControllerTests
    {
        private Mock<IConfiguration> _mockConfig;
        private Mock<IAccountService> _mockAccountService;
        private Mock<IRefreshTokenService> _mockRefreshTokenService;
        private AuthController _controller;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<HttpRequest> _mockHttpRequest;
        private Mock<HttpResponse> _mockHttpResponse;
        private Mock<IResponseCookies> _mockCookie;

        [SetUp]
        public void Setup()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockAccountService = new Mock<IAccountService>();
            _mockRefreshTokenService = new Mock<IRefreshTokenService>();
            _controller = new AuthController(_mockConfig.Object, _mockAccountService.Object, _mockRefreshTokenService.Object);
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpResponse = new Mock<HttpResponse>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockCookie = new Mock<IResponseCookies>();
        }

        // ------------------------------------------------------------------------------------------
        // register test area
        // ------------------------------------------------------------------------------------------
        


        [Test]
        public async Task AddAccount_ReturnsBadRequest_WhenPasswordTooShort()
        {
            // Arrange
            var accountDtos = new AccountDtos("test@example.com", "12345");

            // Act
            var result = await _controller.AddAccount(accountDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Password is too weak, must be greater than 6 characters", badRequestResult.Value);
        }

        [Test]
        public async Task AddAccount_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var accountDtos = new AccountDtos("test@example.com", "1234567");
            _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(new Account());

            // Act
            var result = await _controller.AddAccount(accountDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Email already existed", badRequestResult.Value);
        }

        // ------------------------------------------------------------------------------------------
        // End register test area
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        // Start login test area
        // ------------------------------------------------------------------------------------------
[Test]
        public async Task AddAccount_OK_WhenLoginSuccess()
        {
            
            // Arrange
            var accountDtos = new AccountDtos("string@gmail.com", "string");
            var account = new Account { email = "string@gmail.com", password = "string" };
    
            // Simulate fetching the account from the service by email
            _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
            _mockConfig.Setup(c => c.GetSection("AppSettings:Secret_Key").Value).Returns("thisisthelongpasswordofmyprojectthisisthelongpasswordofmyproject");

            // Create password hash and salt
            CustomEncoder.CreatePasswordHash("string", out byte[] passwordHash, out byte[] passwordSalt);
            account.password = ByteConvertion.ByteArrayToString(passwordHash);
            account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);
            // Mock HttpContext
            

            _mockHttpContext.Setup(ctx => ctx.Response).Returns(_mockHttpResponse.Object);
            _mockHttpContext.Setup(ctx => ctx.Request).Returns(_mockHttpRequest.Object);

            // Set up cookies
            _mockHttpResponse.Setup(res => res.Cookies).Returns(_mockCookie.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };

            // Act
            var result = await _controller.Login(accountDtos);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<OkObjectResult>(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as APIResponser<Dictionary<string, string>>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.success);
            Assert.AreEqual("Login successfully", apiResponse.message);
            Assert.IsTrue(apiResponse.content.ContainsKey("accessToken"));
            Assert.IsTrue(apiResponse.content.ContainsKey("refreshToken"));
        }
        [Test]
        public async Task Login_ReturnsNotFound_WhenEmailNotFound()
        {
            // Arrange
            var accountDtos = new AccountDtos("test@example.com", "12345");
            _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync((Account)null);

            // Act
            var result = await _controller.Login(accountDtos);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Email not found", notFoundResult.Value);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenWrongCredentials()
        {
            // Arrange
            var accountDtos = new AccountDtos("test@example.com", "12345");
            var account = new Account { email = "test@example.com", password = "correcthash" };
            _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
            CustomEncoder.CreatePasswordHash("correctpassword", out byte[] passwordHash, out byte[] passwordSalt);
            account.password = ByteConvertion.ByteArrayToString(passwordHash);
            account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

            // Act
            var result = await _controller.Login(accountDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Wrong credentials", badRequestResult.Value);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenUsingSqlInjection()
        {
            // Arrange
            var accountDtos = new AccountDtos("test@example.com", "12345");
            var account = new Account { email = "test@example.com", password = "correcthash or '1=1" };
            _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
            CustomEncoder.CreatePasswordHash("correctpassword", out byte[] passwordHash, out byte[] passwordSalt);
            account.password = ByteConvertion.ByteArrayToString(passwordHash);
            account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

            // Act
            var result = await _controller.Login(accountDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Wrong credentials", badRequestResult.Value);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenUsingSqlInjection2()
        {
            // Arrange
            var accountDtos = new AccountDtos("test@example.com", "12345");
            var account = new Account { email = "test@example.com", password = "' # correcthash" };
            _mockAccountService.Setup(s => s.GetByEmail(accountDtos.email)).ReturnsAsync(account);
            CustomEncoder.CreatePasswordHash("correctpassword", out byte[] passwordHash, out byte[] passwordSalt);
            account.password = ByteConvertion.ByteArrayToString(passwordHash);
            account.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);

            // Act
            var result = await _controller.Login(accountDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Wrong credentials", badRequestResult.Value);
        }

        // ------------------------------------------------------------------------------------------
        // End login test area
        // ------------------------------------------------------------------------------------------

        // ------------------------------------------------------------------------------------------
        // Start refresh token test area
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
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("Invalid refresh token", unauthorizedResult.Value);
        }
    }
}
