﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
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
        
        private readonly RsaSecurityKey _jwtSecurityKey = null;

        public TestsFixture()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var testIdp = new Dictionary<string, string>()
                    {
                        {"IdentityProviders:0:Name", "Testing"},
                        {"IdentityProviders:0:Issuer", "https://example.com"},
                        {"IdentityProviders:0:Audience", "https://client.example.com"},
                        {"IdentityProviders:0:SkipValidation", "true"},

                        {
                            "ConnectionStrings:mssql",
                            "Connection Type=Memory;Initial Catalog=testing;User=whoami;Password=mysupersecretpassword;"
                        }
                    };

                    builder.AddInMemoryCollection(testIdp);

                    IdentityModelEventSource.ShowPII = true;
                }));

            Client = Server.CreateClient();
            
            _jwtSecurityKey = new RsaSecurityKey(RSA.Create(2048));
            _jwtSecurityKey.KeyId = "000132c6-b5eb-4c7b-9be0-f3a2825fac99";
        }

        public string GetJwt(string sub = null, string givenName = null, string familyName = null,
            string email = null, string audience = null, string issuer = null)
        {
            if (string.IsNullOrWhiteSpace(sub))
                sub = "2bff865f-0c95-4a3a-a12f-3b3f0743d279";

            if (string.IsNullOrWhiteSpace(givenName))
                givenName = "John";

            if (string.IsNullOrWhiteSpace(familyName))
                familyName = "Smith";

            if (string.IsNullOrWhiteSpace(email))
                email = "john.smith@acme.com";

            if (string.IsNullOrWhiteSpace(audience))
                audience = "https://client.example.com";

            if (string.IsNullOrWhiteSpace(issuer))
                issuer = "https://account.example.com";

            SigningCredentials signingCredentials =
                new SigningCredentials(_jwtSecurityKey, SecurityAlgorithms.RsaSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, sub),
                    new Claim(ClaimTypes.GivenName, givenName),
                    new Claim(ClaimTypes.Surname, familyName),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                Issuer = issuer,
                Audience = audience,
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