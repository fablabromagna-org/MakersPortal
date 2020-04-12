using MakersPortal.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MakersPortal.Tests.Integration
{
    public class KeyManagerIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly IntegrationTestsFixture _integrationTestsFixture;

        public KeyManagerIntegrationTests(IntegrationTestsFixture integrationTestsFixture)
        {
            _integrationTestsFixture = integrationTestsFixture;
        }

        [Fact]
        public void EnsureKeyManagerIsRegistered_Success()
        {
            IKeyManager keyManager = _integrationTestsFixture.Server.Services.GetService<IKeyManager>();
            Assert.NotNull(keyManager);
        }
    }
}