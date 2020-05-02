using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
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
    public class KeyManager : IKeyManager
    {
        private readonly AzureKeyVaultOptions _keyVaultOptions;
        private readonly KeysOptions _keys;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyManager(IOptions<AzureKeyVaultOptions> keyVaultOptions, IOptions<KeysOptions> keys, IKeyVaultClient keyVaultClient)
        {
            Debug.Assert(keyVaultOptions != null);
            Debug.Assert(keys != null);
            Debug.Assert(keyVaultClient != null);

            _keyVaultOptions = keyVaultOptions.Value;
            _keys = keys.Value;
            _keyVaultClient = keyVaultClient;
        }

        /// <inheritdoc cref="IKeyManager"/>
        public async Task<RsaSecurityKey> GetSecurityKeyFromNameAsync(string name)
        {
            RSA key = await GetKey(name);
            var securityKey = new RsaSecurityKey(key);

            securityKey.KeyId = _keys[name].Kid;
            
            return securityKey;
        }

        /// <inheritdoc cref="IKeyManager"/>
        public Task<string> SignJwtAsync(IEnumerable<Claim> claims, string issuer, string audience)
        {
            string keyVaultEndopoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");
           // _keyVaultClient.SignAsync(keyVaultEndopoint, "Jwt", _keys["Jwt"].Version, SecurityAlgorithms.Sha256, )
           return Task.FromResult("");
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

        private string CreateJwtHeaders(string algorithm, string kid)
        {
            return String.Empty;
        }
        
        
    }
}