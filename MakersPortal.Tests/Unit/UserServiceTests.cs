using System;
using Bogus;
using MakersPortal.Core.Models;
using MakersPortal.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MakersPortal.Tests.Unit
{
    public class UserServiceTests : IClassFixture<TestsFixture>
    {
        private readonly Faker _faker;
        private readonly UserService _userService;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;

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

            _userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, optionsAccessor.Object,
                passwordHasher.Object, new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0], keyNormalizer.Object, identityErrorDescriber.Object,
                serviceProvider.Object, logger.Object);
            
            _userManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());
            //_userService = new UserService(_userManager.Object);
        }
    }
}