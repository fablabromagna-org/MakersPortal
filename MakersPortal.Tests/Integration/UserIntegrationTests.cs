using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MakersPortal.Tests.Integration
{
    public class UserIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly IntegrationTestsFixture _integrationTestsFixture;

        public UserIntegrationTests(IntegrationTestsFixture integrationTestsFixture)
        {
            _integrationTestsFixture = integrationTestsFixture;
        }
        
        [Fact]
        public async Task EditPersonalDetails_NoCondition_Success()
        {
            HttpResponseMessage response = await _integrationTestsFixture.Client.GetAsync("ddd");
            response.EnsureSuccessStatusCode();
        }
    }
}