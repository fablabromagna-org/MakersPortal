//-------------------------------------------
// Based on Microsoft's code.
// Originally released under MIT license.
//-------------------------------------------

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Infrastructure.AzureKeyVault
{
    /// <summary>
    /// Provides signing and verifying operations using Azure Key Vault.
    /// </summary>
    public class KeyVaultSecurityKey : SecurityKey
    {
        private int? _keySize;
        private string _keyId;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultSecurityKey"/> class.
        /// </summary>
        /// <param name="keyIdentifier">The key identifier that is recognized by KeyVault.</param>
        /// <param name="keyVaultClient">The key vault client instance</param>
        /// <exception cref="ArgumentNullException">if <paramref name="keyIdentifier"/> is null or empty.</exception>
        public KeyVaultSecurityKey(string keyIdentifier, IKeyVaultClient keyVaultClient)
        {
            KeyId = keyIdentifier;
            _keyVaultClient = keyVaultClient;
        }

        private IKeyVaultClient _keyVaultClient;

        /// <summary>
        /// The uniform resource identifier of the security key.
        /// </summary>
        public override string KeyId
        {
            get => _keyId;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw LogHelper.LogArgumentNullException(nameof(value));
                
                if (StringComparer.Ordinal.Equals(_keyId, value))
                    return;

                _keyId = value;

                // Reset the properties so they can be retrieved from Azure KeyVault the next time they are accessed.
                _keySize = null;
            }
        }

        /// <summary>
        /// The size of the security key.
        /// </summary>
        public override int KeySize => _keySize.Value;

        /// <summary>
        /// Retrieve the properties from Azure Key Vault.
        /// </summary>
        public async Task Initialize()
        {
            var bundle = await _keyVaultClient.GetKeyAsync(_keyId, CancellationToken.None);
            _keySize = new BitArray(bundle.Key.N).Length;
        }
    }
}