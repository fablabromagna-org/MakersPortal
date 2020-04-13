using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest.Azure;
using JsonWebKey = Microsoft.IdentityModel.Tokens.JsonWebKey;

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
        private readonly IConfiguration _configuration;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyManager(IConfiguration configuration, IKeyVaultClient keyVaultClient)
        {
            _configuration = configuration;
            _keyVaultClient = keyVaultClient;
        }
        
        /// <inheritdoc cref="IKeyManager"/>
        public async Task<RsaSecurityKey> GetSecurityKeyFromName(string name)
        {
            return new RsaSecurityKey(await GetKey(name));
        }

        private async Task<RSA> GetKey(string name)
        {
            string keyVaultEndopoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");
            RSA key;
            
            if (keyVaultEndopoint != null)
            {
                AzureOperationResponse<KeyBundle> keyBundleResponse =
                    await _keyVaultClient.GetKeyWithHttpMessagesAsync(keyVaultEndopoint, name,
                        _configuration[$"Keys:{name}:Version"]);

                key = keyBundleResponse.Body.Key.ToRSA();
            }
            else
            {
                key = RSA.Create();
                key.ImportRSAPrivateKey(Convert.FromBase64String(_configuration[$"Keys:{name}:Private"]), out _);
            }

            return key;
        }
    }
}