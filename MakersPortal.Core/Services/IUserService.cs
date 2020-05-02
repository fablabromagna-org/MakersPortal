using System.Threading.Tasks;
using MakersPortal.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Core.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a new session for the provided user
        /// </summary>
        /// <param name="user">The internal user</param>
        /// <returns>The encoded Jwt</returns>
        public Task<string> CreateSessionAsync(ApplicationUser user);
    }
}