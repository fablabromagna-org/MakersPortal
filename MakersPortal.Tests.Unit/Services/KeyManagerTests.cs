using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using Bogus;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using MakersPortal.Infrastructure.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest.Azure;
using Moq;
using Xunit;
using JsonWebKey = Microsoft.Azure.KeyVault.WebKey.JsonWebKey;

namespace MakersPortal.Tests.Unit.Services
{
    public class KeyManagerTests
    {
        private readonly KeysOptions _keysOptions;
        private const string KeyName = "Jwt";

        public KeyManagerTests()
        {
            var key = RSA.Create();

            _keysOptions = new KeysOptions()
            {
                Jwt = new KeysOptions.Key()
                {
                    Private = Convert.ToBase64String(key.ExportRSAPrivateKey()), // Local
                    Public = Convert.ToBase64String(key.ExportRSAPublicKey()), // Local
                    Kid = Guid.NewGuid().ToString(), // Both
                    Version = Guid.NewGuid().ToString() // Azure Key Vault
                }
            };
        }

        [Fact]
        public async void GetSecurityKeyFromNameAsync_Success_FromLocalEnvironment()
        {
            var keyVaultOptions = new AzureKeyVaultOptions
            {
                Endpoint = null
            };

            var keyVaultClient = new Mock<IKeyVaultClient>();

            IKeyManager keyManager = new KeyManager(Options.Create(keyVaultOptions), Options.Create(_keysOptions),
                keyVaultClient.Object);

            var responseKey = await keyManager.GetSecurityKeyFromNameAsync(KeyName);

            Assert.NotNull(responseKey);
            Assert.Equal(_keysOptions[KeyName].Kid, responseKey.KeyId);
            Assert.Equal(_keysOptions[KeyName].Private, Convert.ToBase64String(responseKey.Rsa.ExportRSAPrivateKey()));
        }

        [Fact]
        public async void GetSecurityKeyFromNameAsync_Success_FromAzureKeyVault()
        {
            var faker = new Faker();
            var keyVaultOptions = new AzureKeyVaultOptions
            {
                Endpoint = faker.Internet.Url()
            };

            var keyVaultClient = new Mock<IKeyVaultClient>();

            var encodedKey = RSA.Create();
            encodedKey.ImportRSAPrivateKey(Convert.FromBase64String(_keysOptions.Jwt.Private), out _);

            keyVaultClient.Setup(client => client.GetKeyWithHttpMessagesAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), null, CancellationToken.None)).ReturnsAsync(
                new AzureOperationResponse<KeyBundle>()
                {
                    Body = new KeyBundle()
                    {
                        Key = new JsonWebKey(encodedKey)
                    }
                });

            IKeyManager keyManager = new KeyManager(Options.Create(keyVaultOptions), Options.Create(_keysOptions),
                keyVaultClient.Object);

            RsaSecurityKey response = await keyManager.GetSecurityKeyFromNameAsync(KeyName);
            Assert.NotNull(response);
            Assert.Equal(_keysOptions.Jwt.Kid, response.KeyId);
        }

        [Fact]
        public async void SignJwtAsync_Success_FromLocalEnvironment()
        {
            var keyVaultOptions = new AzureKeyVaultOptions
            {
                Endpoint = null
            };

            var keyVaultClient = new Mock<IKeyVaultClient>();

            IKeyManager keyManager = new KeyManager(Options.Create(keyVaultOptions), Options.Create(_keysOptions),
                keyVaultClient.Object);

            var jwt = BuildClaims();

            string signedToken = await keyManager.SignJwtAsync(claims: BuildClaims(), issuer: "https://example.com",
                audience: "https://client.example.com", notBefore: DateTime.Now, expires: DateTime.Now.AddDays(15));

            Assert.NotNull(signedToken);
            Assert.NotEmpty(signedToken);
        }

        [Fact]
        public async void SignJwtAsync_Success_FromAzureKeyVault()
        {
            IdentityModelEventSource.ShowPII = true;
            var faker = new Faker();
            var keyVaultOptions = new AzureKeyVaultOptions
            {
                Endpoint = faker.Internet.Url()
            };

            var keyVaultClient = new Mock<IKeyVaultClient>();

            var encodedKey = RSA.Create();
            encodedKey.ImportRSAPrivateKey(Convert.FromBase64String(_keysOptions.Jwt.Private), out _);

            keyVaultClient.Setup(client => client.GetKeyWithHttpMessagesAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), null, CancellationToken.None)).ReturnsAsync(
                new AzureOperationResponse<KeyBundle>()
                {
                    Body = new KeyBundle()
                    {
                        Key = new JsonWebKey(encodedKey)
                    }
                });

            keyVaultClient.Setup(client => client.SignWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), null, CancellationToken.None))
                .ReturnsAsync((string vaultBaseUrl, string keyName, string keyVersion, string algorithm, byte[] data,
                    Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken) =>
                {
                    var cryptoServiceProvider = new RSACryptoServiceProvider();
                    cryptoServiceProvider.ImportRSAPrivateKey(Convert.FromBase64String(_keysOptions.Jwt.Private),
                        out _);

                    var signedData = cryptoServiceProvider.SignData(data, CryptoConfig.MapNameToOID("SHA256"));

                    return new AzureOperationResponse<KeyOperationResult>()
                    {
                        Body = new KeyOperationResult($"{vaultBaseUrl}/keys/{keyName}/{keyVersion}", signedData)
                    };
                });

            IKeyManager keyManager = new KeyManager(Options.Create(keyVaultOptions), Options.Create(_keysOptions),
                keyVaultClient.Object);

            string signedToken = await keyManager.SignJwtAsync(claims: BuildClaims(), issuer: "https://example.com",
                audience: "https://client.example.com", notBefore: DateTime.Now, expires: DateTime.Now.AddDays(15));

            Assert.NotNull(signedToken);
            Assert.NotEmpty(signedToken);
        }

        private IEnumerable<Claim> BuildClaims()
        {
            var faker = new Faker();
            var id = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, faker.Person.Email),
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.GivenName, faker.Person.FirstName),
                new Claim(ClaimTypes.Surname, faker.Person.LastName),
                new Claim(ClaimTypes.Role, "Developer"),
                new Claim(ClaimTypes.Role, "Sales"),
            };

            return claims;
        }
    }
}