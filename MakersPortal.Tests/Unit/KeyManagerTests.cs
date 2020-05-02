using System;
using System.Security.Cryptography;
using System.Threading;
using Bogus;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using MakersPortal.Infrastructure.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest.Azure;
using Moq;
using Xunit;
using JsonWebKey = Microsoft.Azure.KeyVault.WebKey.JsonWebKey;

namespace MakersPortal.Tests.Unit
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
    }
}