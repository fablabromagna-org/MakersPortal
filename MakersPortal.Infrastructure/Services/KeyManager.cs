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
        private readonly IMapper _mapper;

        public KeyManager(IConfiguration configuration, IKeyVaultClient keyVaultClient, IMapper mapper)
        {
            _configuration = configuration;
            _keyVaultClient = keyVaultClient;
            _mapper = mapper;
        }

        /// <inheritdoc cref="IKeyManager"/>
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

            var key = keyBundleResponse.Body.Key;
            JwkDto jwk = new JwkDto
            {
                Kid = _configuration[$"Keys:{name}:Kid"],
                Alg = JsonWebKeySignatureAlgorithm.RS256,
                Use = JsonWebKeyUseNames.Sig,
                Kty = key.Kty,
                E = WebEncoders.Base64UrlEncode(key.E),
                N = WebEncoders.Base64UrlEncode(key.N)
            };

            return jwk;
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