using System.Threading.Tasks;
using MakersPortal.Core.Dtos;

namespace MakersPortal.Core.Services
{
    public interface IKeyManager
    {
        /// <summary>
        /// Retrieves a key from Azure Key Vault (or from secret manager, if not available)
        /// </summary>
        /// <param name="name">The key name</param>
        /// <returns>The public key</returns>
        public Task<JwkDto> GetPublicFromName(string name);
    }
}