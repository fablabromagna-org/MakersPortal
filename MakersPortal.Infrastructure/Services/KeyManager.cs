using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Services;
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
    // when in testing environment, for secret manager
    // Beside it's own nature, it's difficult to test in a public environment
    [ExcludeFromCodeCoverage]
    public class KeyManager : IKeyManager
    {
        private readonly IConfiguration _configuration;
        private readonly IKeyVaultClient _keyVaultClient;
        private readonly IMapper _mapper;

        public KeyManager(IConfiguration configuration, IKeyVaultClient keyVaultClient, IMapper mapper)
        {
            _configuration = configuration;
            _keyVaultClient = keyVaultClient;
            _mapper = mapper;
        }

        public async Task<JwkDto> GetPublicFromName(string name)
        {
            string keyVaultEndopoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

            if (keyVaultEndopoint != null)
            {
                return await GetKeyFromAzureKeyVault(name, keyVaultEndopoint);
            }

            return GetKeyFromLocalEnvironment(name);
        }

        private async Task<JwkDto> GetKeyFromAzureKeyVault(string name, string keyVaultEndopoint)
        {
            AzureOperationResponse<KeyBundle> keyBundleResponse =
                await _keyVaultClient.GetKeyWithHttpMessagesAsync(keyVaultEndopoint, name,
                    _configuration[$"Keys:{name}:Version"]);

            return _mapper.Map<JwkDto>(keyBundleResponse.Body.Key);
        }

        private JwkDto GetKeyFromLocalEnvironment(string name)
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_configuration[$"Keys:{name}:Private"]), out _);

            JsonWebKey jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa));
            jwk.Kid = _configuration[$"Keys:{name}:Kid"];
            jwk.Alg = JsonWebKeySignatureAlgorithm.RS256;
            jwk.Use = JsonWebKeyUseNames.Sig;

            return _mapper.Map<JwkDto>(jwk);
        }
    }
}