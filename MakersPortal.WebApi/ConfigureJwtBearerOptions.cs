using MakersPortal.Core.Dtos.Configuration;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IKeyManager _keyManager;
        private readonly JwtIssuerDto _issuer;

        public ConfigureJwtBearerOptions(IKeyManager keyManager, JwtIssuerDto issuer)
        {
            _keyManager = keyManager;
            _issuer = issuer;
        }

        public void Configure(JwtBearerOptions options)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _keyManager.GetSecurityKeyFromName("jwt").Result,

                ValidIssuer = _issuer.Issuer,
                ValidateIssuer = true
            };

            options.Audience = _issuer.Audience;
            options.SaveToken = true;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }
}