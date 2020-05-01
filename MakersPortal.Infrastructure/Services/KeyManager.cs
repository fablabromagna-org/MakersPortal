using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest.Azure;

namespace MakersPortal.Infrastructure.Services
{
    // ToDo: Find a way to test IKeyManager interface
    // IKeyManager is a wrapper for Azure Key Vault and,
    // when in the testing environment, for secret manager
    // Due its own nature, it's difficult to test
    // Keeping a To do but excluding from code coverage
    [ExcludeFromCodeCoverage]
    public class KeyManager : IKeyManager
    {
        private readonly KeysOptions _keys;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyManager(IOptions<KeysOptions> keys, IKeyVaultClient keyVaultClient)
        {
            Debug.Assert(keys != null);
            Debug.Assert(keyVaultClient != null);

            _keys = keys.Value;
            _keyVaultClient = keyVaultClient;
        }

        /// <inheritdoc cref="IKeyManager"/>
        public async Task<RsaSecurityKey> GetSecurityKeyFromName(string name)
        {
            RSA key = await GetKey(name);
            var securityKey = new RsaSecurityKey(key);

            securityKey.KeyId = _keys[name].Kid;
            
            return securityKey;
        }

        private async Task<RSA> GetKey(string name)
        {
            string keyVaultEndopoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");
            RSA key;

            if (keyVaultEndopoint != null)
            {
                AzureOperationResponse<KeyBundle> keyBundleResponse =
                    await _keyVaultClient.GetKeyWithHttpMessagesAsync(keyVaultEndopoint, name,
                        _keys[name].Version);

                key = keyBundleResponse.Body.Key.ToRSA();
            }
            else
            {
                key = RSA.Create();
                key.ImportRSAPrivateKey(Convert.FromBase64String(_keys[name].Private), out _);
            }

            return key;
        }
    }
}