using MakersPortal.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MakersPortal.Tests.Integration
{
    public class UserServiceIntegrationTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public UserServiceIntegrationTests(TestsFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void EnsureUserServiceIsRegistered_Success()
        {
            var userService = _fixture.Server.Services.GetService<IUserService>();
            Assert.NotNull(userService);
        }
    }
}