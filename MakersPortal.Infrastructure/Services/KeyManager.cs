using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.AzureKeyVault;
using MakersPortal.Infrastructure.Options;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using KeyVaultSecurityKey = MakersPortal.Infrastructure.AzureKeyVault.KeyVaultSecurityKey;

namespace MakersPortal.Infrastructure.Services
{
    public class KeyManager : IKeyManager
    {
        private readonly AzureKeyVaultOptions _keyVaultOptions;
        private readonly KeysOptions _keys;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyManager(IOptions<AzureKeyVaultOptions> keyVaultOptions, IOptions<KeysOptions> keys,
            IKeyVaultClient keyVaultClient)
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
        public async Task<string> SignJwtAsync(IEnumerable<Claim> claims, string issuer, string audience, DateTime notBefore,
            DateTime expires)
        {
            const string keyName = "Jwt";

            SigningCredentials signingCredentials;

            if (!string.IsNullOrEmpty(_keyVaultOptions.Endpoint))
            {
                var keyVaultSecurityKey =
                    new KeyVaultSecurityKey($"{_keyVaultOptions.Endpoint}/keys/{keyName}/{_keys[keyName].Version}",
                        _keyVaultClient);
                await keyVaultSecurityKey.Initialize();

                signingCredentials = new SigningCredentials(keyVaultSecurityKey, SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory()
                        {CustomCryptoProvider = new AzureKeyVaultCryptoProvider(_keyVaultClient)}
                };
            }
            else
            {
                var key = RSA.Create();
                key.ImportRSAPrivateKey(Convert.FromBase64String(_keys[keyName].Private), out _);

                var securityKey = new RsaSecurityKey(key);
                securityKey.KeyId = _keys[keyName].Kid;

                signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            }

            var signingToken = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims,
                expires: expires, notBefore: notBefore, signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(signingToken);
        }

        private async Task<RSA> GetKey(string name)
        {
            RSA key;

            if (_keyVaultOptions.Endpoint != null)
            {
                KeyBundle keyResponse =
                    await _keyVaultClient.GetKeyAsync(_keyVaultOptions.Endpoint, name, _keys[name].Version);
                
                key = keyResponse.Key.ToRSA();
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