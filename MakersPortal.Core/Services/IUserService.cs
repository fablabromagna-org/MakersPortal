using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Core.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Verifies against every configured Identity Provider the Jwt Token
        /// </summary>
        /// <param name="tokenDto">The Dto of the token</param>
        /// <returns>Null if none of the configured Identity Providers trusts the identity, the validated token otherwise</returns>
        public Task<JwtSecurityToken> ValidateExternalJwtToken(JwtTokenDto tokenDto);

        /// <summary>
        /// Find a user from the email address in the Jwt token
        /// </summary>
        /// <param name="externalIdpJwt">The Jwt token</param>
        /// <returns>The user if found, null otherwise</returns>
        public ApplicationUser Find(SecurityToken externalIdpJwt);

        /// <summary>
        /// Creates a new session for the provided user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="externalIdpJwt">The Jwt token from the external identity provider</param>
        /// <returns>A new Jwt token for this Apis</returns>
        public SecurityToken CreateSession(ApplicationUser user, SecurityToken externalIdpJwt);

        /// <summary>
        /// Creates a new user from the provided external Idp's Jwt token
        /// </summary>
        /// <param name="externalIdpJwt">The Jwt token from the external provider</param>
        /// <returns>The new user</returns>
        public ApplicationUser Create(SecurityToken externalIdpJwt);
    }
}