using System;
using System.Security.Cryptography;
using System.Threading;
using Bogus;
using MakersPortal.Infrastructure.AzureKeyVault;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Rest.Azure;
using Moq;
using Xunit;

namespace MakersPortal.Tests.Unit.Services
{
    public class KeyVaultSecurityKeyTests
    {
        private readonly Faker _faker;
        private readonly string _keyId;
        private readonly Mock<IKeyVaultClient> _keyVaultClient;
        private readonly RSA _rsaKey;

        public KeyVaultSecurityKeyTests()
        {
            _faker = new Faker();
            _rsaKey = RSA.Create();
            _keyId = $"{_faker.Internet.Url()}/keys/Jwt/{Guid.NewGuid().ToString()}";
            _keyVaultClient = new Mock<IKeyVaultClient>();

            var jwk = new JsonWebKey(_rsaKey);
            jwk.Kid = _keyId;

            _keyVaultClient.Setup(client =>
                client.GetKeyWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null,
                    CancellationToken.None)).ReturnsAsync(new AzureOperationResponse<KeyBundle>
            {
                Body = new KeyBundle()
                {
                    Key = jwk
                }
            });
        }

        [Fact]
        public void KeySize_InvalidOperationException_WhenIsNotInitialized()
        {
            var keyVaultSecurityKey = new KeyVaultSecurityKey(_keyId, _keyVaultClient.Object);

            Assert.Throws<InvalidOperationException>(() => keyVaultSecurityKey.KeySize);
        }

        [Fact]
        public void KeyId_Success_WhenValueIsRetrieved()
        {
            var keyVaultSecurityKey = new KeyVaultSecurityKey(_keyId, _keyVaultClient.Object);

            Assert.Equal(_keyId, keyVaultSecurityKey.KeyId);
        }

        [Fact]
        public void KeyId_ThrowsArgumentNullException_WhenKeyIdIsSettedNullOrEmptyString()
        {
            var keyVaultSecurityKey = new KeyVaultSecurityKey(_keyId, _keyVaultClient.Object);

            Assert.Throws<ArgumentNullException>(() => keyVaultSecurityKey.KeyId = null);
            Assert.Throws<ArgumentNullException>(() => keyVaultSecurityKey.KeyId = String.Empty);
        }

        [Fact]
        public void KeyId_Success_WhenTheKeyIsUpdated()
        {
            var keyVaultSecurityKey = new KeyVaultSecurityKey(_keyId, _keyVaultClient.Object);

            var newKey = $"{_faker.Internet.Url()}/keys/Jwt/{Guid.NewGuid().ToString()}";

            keyVaultSecurityKey.KeyId = newKey;
            Assert.Equal(newKey, keyVaultSecurityKey.KeyId);
            Assert.Throws<InvalidOperationException>(() =>
                keyVaultSecurityKey.KeySize); // _keySize is null, _keySize.Value does not exists
        }

        [Fact]
        public async void Initialize_Success_NoCondition()
        {
            var keyVaultSecurityKey = new KeyVaultSecurityKey(_keyId, _keyVaultClient.Object);

            var newKey = $"{_faker.Internet.Url()}/keys/Jwt/{Guid.NewGuid().ToString()}";

            await keyVaultSecurityKey.Initialize();
            
            Assert.Equal(_rsaKey.KeySize, keyVaultSecurityKey.KeySize);
            Assert.Equal(_keyId, keyVaultSecurityKey.KeyId);
        }
    }
}