using System.Threading.Tasks;
using MakersPortal.Core.Models;

namespace MakersPortal.Core.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a new session to the specified user
        /// </summary>
        /// <param name="user">the user you want create a new session</param>
        /// <returns>the Jwt token</returns>
        public Task<string> CreateSessionAsync(ApplicationUser user);
    }
}