using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
/*
        [Fact]
        public async Task EditPersonalDetails_NoCondition_Success()
        {
           /* _integrationTestsFixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _integrationTestsFixture.GetJwt());
            HttpResponseMessage response = await _integrationTestsFixture.Client.GetAsync("/Account");
            response.EnsureSuccessStatusCode();
        }
        
        */
    }
}