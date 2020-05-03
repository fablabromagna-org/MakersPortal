//------------------------------------------------------------------------------
// Based on Microsoft's code
// Originally released with MIT license
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Infrastructure.AzureKeyVault
{
    /// <summary>
    /// Provides cryptographic operators based on Azure Key Vault.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AzureKeyVaultCryptoProvider : ICryptoProvider
    {
        private readonly CryptoProviderCache _cache;
        private readonly IKeyVaultClient _keyVaultClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultCryptoProvider"/> class.
        /// </summary>
        public AzureKeyVaultCryptoProvider(IKeyVaultClient keyVaultClient)
        {
            _cache = new InMemoryCryptoProviderCache();
            _keyVaultClient = keyVaultClient;
        }

        /// <summary>
        /// Returns a cryptographic operator that supports the algorithm.
        /// </summary>
        /// <param name="algorithm">the algorithm that defines the cryptographic operator.</param>
        /// <param name="args">the arguments required by the cryptographic operator. May be null.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="algorithm"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="args"/> is null.</exception>
        /// <exception cref="NotSupportedException">if <paramref name="args"/> does not contain a <see cref="KeyVaultSecurityKey"/>.</exception>
        /// <remarks>call <see cref="ICryptoProvider.Release(object)"/> when finished with the object.</remarks>
        public object Create(string algorithm, params object[] args)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw LogHelper.LogArgumentNullException(nameof(algorithm));

            if (args == null)
                throw LogHelper.LogArgumentNullException(nameof(args));

            if (args.FirstOrDefault() is KeyVaultSecurityKey key)
            {
                if (JsonWebKeyEncryptionAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal))
                    return new KeyVaultKeyWrapProvider(key, algorithm);
                
                if (JsonWebKeySignatureAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal))
                {
                    var willCreateSignatures = (bool) (args.Skip(1).FirstOrDefault() ?? false);

                    if (_cache.TryGetSignatureProvider(key, algorithm, typeofProvider: key.GetType().ToString(),
                        willCreateSignatures, out var cachedProvider))
                        return cachedProvider;

                    var signatureProvider = new AzureKeyVaultSignatureProvider(key, algorithm, willCreateSignatures, _keyVaultClient);
                    _cache.TryAdd(signatureProvider);
                    return signatureProvider;
                }
            }

            throw LogHelper.LogExceptionMessage(
                new NotSupportedException(LogHelper.FormatInvariant("Algorithm not supported.", algorithm)));
        }

        /// <summary>
        /// Called to determine if a cryptographic operation is supported.
        /// </summary>
        /// <param name="algorithm">the algorithm that defines the cryptographic operator.</param>
        /// <param name="args">the arguments required by the cryptographic operator. May be null.</param>
        /// <returns>true if supported</returns>
        public bool IsSupportedAlgorithm(string algorithm, params object[] args)
        {
            if (string.IsNullOrEmpty(algorithm))
                throw LogHelper.LogArgumentNullException(nameof(algorithm));

            if (args == null)
                throw LogHelper.LogArgumentNullException(nameof(args));

            return args.FirstOrDefault() is KeyVaultSecurityKey
                   && (JsonWebKeyEncryptionAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal) ||
                       JsonWebKeySignatureAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal));
        }

        /// <summary>
        /// Called to release the object returned from <see cref="ICryptoProvider.Create(string, object[])"/>
        /// </summary>
        /// <param name="cryptoInstance">the object returned from <see cref="ICryptoProvider.Create(string, object[])"/>.</param>
        public void Release(object cryptoInstance)
        {
            if (cryptoInstance is SignatureProvider signatureProvider)
                _cache.TryRemove(signatureProvider);

            if (cryptoInstance is IDisposable obj)
                obj.Dispose();
        }
    }
}