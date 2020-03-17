using System;
using System.Net.Http;
using MakersPortal.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace MakersPortal.Tests.Integration
{
    public class IntegrationTestsFixture : IDisposable
    {
        public HttpClient Client;
        private readonly TestServer _server;
        
        public IntegrationTestsFixture()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = _server.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}