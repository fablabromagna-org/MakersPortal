using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using MakersPortal.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly HttpClient Client;
        public readonly TestServer Server;

        // Random (and not secure) Jwt security key
        // For testing purpose only
        private readonly SymmetricSecurityKey _jwtSecurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

        public TestsFixture()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>().ConfigureAppConfiguration((context,
                builder) =>
            {
                var testIdp = new Dictionary<string, string>()
                {
                    {"IdentityProviders:0:Name", "Testing"},
                    {"IdentityProviders:0:Issuer", "https://example.com"},
                    {"IdentityProviders:0:Audience", "https://example.com"},
                    {"IdentityProviders:0:SkipValidation", "true"}
                };

                builder.AddInMemoryCollection(testIdp);

                IdentityModelEventSource.ShowPII = true;
            }));

            Client = Server.CreateClient();
        }

        public string GetJwt(string sub = null, string givenName = null, string familyName = null, string email = null)
        {
            if (string.IsNullOrWhiteSpace(sub))
                sub = "2bff865f-0c95-4a3a-a12f-3b3f0743d279";

            if (string.IsNullOrWhiteSpace(givenName))
                givenName = "John";

            if (string.IsNullOrWhiteSpace(familyName))
                familyName = "Smith";

            if (string.IsNullOrWhiteSpace(email))
                email = "john.smith@acme.com";

            SigningCredentials signingCredentials =
                new SigningCredentials(_jwtSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, sub),
                    new Claim(ClaimTypes.GivenName, givenName),
                    new Claim(ClaimTypes.Surname, familyName),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                Issuer = "example.com",
                Audience = "https://example.com",
                SigningCredentials = signingCredentials
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}