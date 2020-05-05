using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Core.Services
{
    public interface IKeyManager
    {
        /// <summary>
        /// Retrieves a key from Azure Key Vault (or from secret manager, if not available)
        /// </summary>
        /// <param name="name">The key name</param>
        /// <returns>The public key</returns>
        public Task<RsaSecurityKey> GetSecurityKeyFromNameAsync(string name);

        public Task<string> SignJwtAsync(IEnumerable<Claim> claims, string issuer, string audience, DateTime notBefore,
            DateTime expires);
    }
}