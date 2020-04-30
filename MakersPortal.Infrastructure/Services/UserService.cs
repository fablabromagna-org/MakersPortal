using System.Collections.Generic;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IEnumerable<IdentityProvider> _identityProviders;

        public UserService(IOptions<IdentityProvidersOptions> identityProviders)
        {
            _identityProviders = identityProviders.Value.IdentityProviders;
        }

        public bool ValidateExternalJwtToken(JwtTokenDto tokenDto, out SecurityToken externalIdpValidatedToken)
        {
           externalIdpValidatedToken = null;

           /* if (string.IsNullOrWhiteSpace(tokenDto.Token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();

            foreach (IdentityProviderDto idp in _identityProviders)
            {
                try
                {
                    var claims = tokenHandler.ValidateToken(tokenDto.Token,
                        await GetValidationParameters(false, idp.Issuer, idp.Audience), out externalIdpValidatedToken);

                    if (externalIdpValidatedToken == null)
                        return false;

                    var requiredClaims = new string[]
                        {ClaimTypes.Email, ClaimTypes.GivenName, ClaimTypes.Surname, ClaimTypes.NameIdentifier};

                    if (claims.Claims.Select(p => p.Type).Intersect(requiredClaims).Count() != requiredClaims.Count())
                        return false;

                    return true;
                }
                catch
                {
                    //
                }
            }
*/
            return false;
        }
/*
        private async Task<TokenValidationParameters> GetValidationParameters(bool performValidation, string issuer,
            string audience)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateAudience = performValidation,
                ValidateIssuer = performValidation,
                ValidateLifetime = performValidation,
                ValidateTokenReplay = performValidation,
                RequireAudience = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKeys = await GetJwks(issuer)
            };

            return validationParameters;
        }

        private async Task<IEnumerable<JsonWebKey>> GetJwks(string authority)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(authority);

            var response = await httpClient.GetAsync("/.well-known/openid-configuration");
        }*/

        public ApplicationUser Find(SecurityToken externalIdpJwt)
        {
            throw new System.NotImplementedException();
        }

        public SecurityToken CreateSession(ApplicationUser user, SecurityToken externalIdpJwt)
        {
            throw new System.NotImplementedException();
        }

        public ApplicationUser Create(SecurityToken externalIdpJwt)
        {
            throw new System.NotImplementedException();
        }
    }
}