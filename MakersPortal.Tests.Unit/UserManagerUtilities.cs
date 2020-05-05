using System;
using System.Diagnostics.CodeAnalysis;
using MakersPortal.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MakersPortal.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    public abstract class UserManagerUtilities<TUser, TKey>
        where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        public static Mock<UserManager<TUser>> CreateMockedUserManager()
        {
            var userStore = new Mock<IUserStore<TUser>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<TUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var identityErrorDescriber = new Mock<IdentityErrorDescriber>();
            var serviceProvider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<TUser>>>();

            var userManager = new Mock<UserManager<TUser>>(userStore.Object, optionsAccessor.Object,
                passwordHasher.Object, new IUserValidator<TUser>[0],
                new IPasswordValidator<TUser>[0], keyNormalizer.Object, identityErrorDescriber.Object,
                serviceProvider.Object, logger.Object);

            return userManager;
        }
    }

    [ExcludeFromCodeCoverage]
    public sealed class UserManagerUtilities : UserManagerUtilities<ApplicationUser, string>
    {
    }
}