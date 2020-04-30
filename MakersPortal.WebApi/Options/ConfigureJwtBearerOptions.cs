using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi.Options
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IKeyManager _keyManager;
        private readonly JwtIssuerOptions _issuer;

        public ConfigureJwtBearerOptions(IKeyManager keyManager, IOptions<JwtIssuerOptions> issuer)
        {
            _keyManager = keyManager;
            _issuer = issuer.Value;
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