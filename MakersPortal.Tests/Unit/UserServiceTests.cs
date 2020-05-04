using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using MakersPortal.Core.Exceptions;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using MakersPortal.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MakersPortal.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly Faker _faker;
        private readonly UserService _userService;
        private readonly JwtIssuerOptions _issuerOptions;
        private readonly ApplicationUser _testUser;
        private readonly IList<string> _roles;
        private DateTime _nbfDate;
        private DateTime _expDate;

        public UserServiceTests()
        {
            _faker = new Faker();

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var identityErrorDescriber = new Mock<IdentityErrorDescriber>();
            var serviceProvider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, optionsAccessor.Object,
                passwordHasher.Object, new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0], keyNormalizer.Object, identityErrorDescriber.Object,
                serviceProvider.Object, logger.Object);

            _testUser = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "john@example.com",
                Surname = _faker.Person.LastName,
                GivenName = _faker.Person.FirstName
            };

            _roles = new List<string>()
            {
                "Administrator", "Maker"
            };

            userManager.Setup(usrManager =>
                    usrManager.FindByEmailAsync(It.Is<string>(email => email == "john@example.com")))
                .ReturnsAsync(_testUser);

            userManager.Setup(usrManager =>
                    usrManager.FindByEmailAsync(It.Is<string>(email => email != "john@example.com")))
                .Returns(Task.FromResult<ApplicationUser>(null));

            userManager.Setup(usrManager => usrManager.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(_roles);

            Mock<IKeyManager> keyManager = new Mock<IKeyManager>();
            keyManager.Setup(keyMng => keyMng.SignJwtAsync(It.IsAny<IEnumerable<Claim>>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((IEnumerable<Claim> claims, string issuer, string audience, DateTime nbf, DateTime exp) =>
                {
                    _nbfDate = nbf;
                    _expDate = exp;

                    // We are not checking the token..
                    return new JwtPayload(issuer, audience, claims, nbf, exp).Base64UrlEncode();
                });

            _issuerOptions = new JwtIssuerOptions()
            {
                Issuer = "https://example.com",
                Audience = "https://client.example.com"
            };

            _userService = new UserService(userManager.Object, keyManager.Object, Options.Create(_issuerOptions));
        }

        [Fact]
        public async void CreateSessionAsync_ThrowsArgumentNullException_WhenArgumentsAreNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _userService.CreateSessionAsync(null));
        }

        [Fact]
        public async void CreateSessionAsync_ThrowsUserNotFoundException_WhenUserNotExists()
        {
            var user = new ApplicationUser()
            {
                Email = "notfound@example.com"
            };

            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _userService.CreateSessionAsync(user));
        }

        [Fact]
        public async void CreateSessionAsync_Success_WhenUserExists()
        {
            var user = new ApplicationUser()
            {
                Email = "john@example.com"
            };

            var token = await _userService.CreateSessionAsync(user);
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var jwt = JwtPayload.Base64UrlDeserialize(token);

            Assert.Equal((Int32) _nbfDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                jwt.Nbf.Value);
            Assert.Equal((Int32) _expDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                jwt.Exp.Value);

            const int requiredClaims = 4;
            const int userClaims = 5;

            Assert.Equal(requiredClaims + userClaims + _roles.Count(), jwt.Claims.Count());
        }
    }
}