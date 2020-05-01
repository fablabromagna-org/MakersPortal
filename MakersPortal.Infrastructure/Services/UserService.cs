using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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

        public async Task<JwtSecurityToken> ValidateExternalJwtToken(JwtTokenDto tokenDto)
        {
            SecurityToken externalIdpValidatedToken = null;

            if (string.IsNullOrWhiteSpace(tokenDto.Token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            foreach (IdentityProvider idp in _identityProviders)
            {
                try
                {
                    var validationParamenters = await GetValidationParameters(false, idp.Issuer, idp.Audience);
                    var claims = tokenHandler.ValidateToken(tokenDto.Token, validationParamenters,
                        out externalIdpValidatedToken);

                    if (externalIdpValidatedToken == null)
                        return null;

                    var requiredClaims = new string[]
                        {ClaimTypes.Email, ClaimTypes.GivenName, ClaimTypes.Surname, ClaimTypes.NameIdentifier};

                    if (claims.Claims.Select(p => p.Type).Intersect(requiredClaims).Count() != requiredClaims.Count())
                        return null;

                    return null;
                }
                catch
                {
                    //
                }
            }

            return null;
        }

        private async Task<TokenValidationParameters> GetValidationParameters(bool performValidation, string issuer,
            string audience)
        {
            var validationParameters = new TokenValidationParameters
            {
                
                IssuerSigningKeys = await GetJwks(issuer)
            };

            return validationParameters;
        }

        private async Task<IEnumerable<SecurityKey>> GetJwks(string authority)
        {
            ICollection<SecurityKey> signingKeys = null;

            try
            {
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    authority + "/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());

                OpenIdConnectConfiguration discoveryDocument = await configurationManager.GetConfigurationAsync();
                signingKeys = discoveryDocument.SigningKeys;
            }
            catch
            {
                //
            }

            return signingKeys ?? new SecurityKey[] {CreateRandomKey()};
        }

        private SecurityKey CreateRandomKey()
        {
            RsaSecurityKey key = new RsaSecurityKey(RSA.Create());
            key.KeyId = "000132c6-b5eb-4c7b-9be0-f3a2825fac99";

            return key;
        }

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