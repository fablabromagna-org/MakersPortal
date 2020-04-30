using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi
{
    public class ExternalJwtSecurityTokenHandler : ISecurityTokenValidator
    {
        public readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public ExternalJwtSecurityTokenHandler()
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanReadToken(string securityToken) => _jwtSecurityTokenHandler.CanReadToken(securityToken);


        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            throw new System.NotImplementedException();
        }

        public bool CanValidateToken => _jwtSecurityTokenHandler.CanValidateToken;

        public int MaximumTokenSizeInBytes
        {
            get => _jwtSecurityTokenHandler.MaximumTokenSizeInBytes;
            set => _jwtSecurityTokenHandler.MaximumTokenSizeInBytes = value;
        }
    }
}