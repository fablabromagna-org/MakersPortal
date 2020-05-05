using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Exceptions.HttpExceptions;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MakersPortal.Tests.Unit.Controllers
{
    public class AuthControllerTests
    {
        private readonly Faker _faker;
        private readonly ControllerContext _loginHttpContext;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<ILogger> _logger;
        private readonly AuthController _controller;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;

        public AuthControllerTests()
        {
            _faker = new Faker();

            _logger = new Mock<ILogger>();
            _userService = new Mock<IUserService>();

            _loginHttpContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, _faker.Person.Email),
                        new Claim(ClaimTypes.GivenName, _faker.Person.FirstName),
                        new Claim(ClaimTypes.Surname, _faker.Person.LastName),
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    }))
                }
            };

            _userManager = UserManagerUtilities.CreateMockedUserManager();

            _controller = new AuthController(_userService.Object, _userManager.Object, _logger.Object);
        }

        [Fact]
        public async void Login_Success_WhenUserNotExists()
        {
            _controller.ControllerContext = _loginHttpContext;

            _userManager.Setup(usrManager => usrManager.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<ApplicationUser>(null));

            _userManager.Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            const string testToken = "My Super Secret Jwt Token";
            _userService.Setup(userService => userService.CreateSessionAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(testToken);

            var response = await _controller.Login() as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            Assert.NotNull(response.Value);
            Assert.IsType<JwtTokenDto>(response.Value);
            Assert.Equal(testToken, ((JwtTokenDto) response.Value).Token);
        }

        [Fact]
        public async void Login_ReturnsProblem_WhenUserNotExists()
        {
            _controller.ControllerContext = _loginHttpContext;

            _userManager.Setup(usrManager => usrManager.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<ApplicationUser>(null));

            _userManager.Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed());

            const string testToken = "My Super Secret Jwt Token";
            _userService.Setup(userService => userService.CreateSessionAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(testToken);

            await Assert.ThrowsAsync<InternalServerErrorException>(async () => await _controller.Login());
        }

        [Fact]
        public async void Login_Success_WhenUserExists()
        {
            _controller.ControllerContext = _loginHttpContext;

            _userManager.Setup(usrManager => usrManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser
                {
                    UserName = _controller.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
                    Email = _controller.User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value,
                    GivenName = _controller.User.Claims.First(claim => claim.Type == ClaimTypes.GivenName).Value,
                    Surname = _controller.User.Claims.First(claim => claim.Type == ClaimTypes.Surname).Value,
                    EmailConfirmed = true
                });

            const string testToken = "My Super Secret Jwt Token";
            _userService.Setup(userService => userService.CreateSessionAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(testToken);

            var response = await _controller.Login() as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            Assert.NotNull(response.Value);
            Assert.IsType<JwtTokenDto>(response.Value);
            Assert.Equal(testToken, ((JwtTokenDto) response.Value).Token);
        }
    }
}