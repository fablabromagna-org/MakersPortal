using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
            Debug.Assert(keyManager != null);
            Debug.Assert(issuer != null);

            _keyManager = keyManager;
            _issuer = issuer.Value;
        }

        public void Configure(JwtBearerOptions options)
        {
            var key = Task.Run(async () => await _keyManager.GetSecurityKeyFromNameAsync("Jwt"))
                .GetAwaiter().GetResult();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidIssuer = _issuer.Issuer,
                ValidateIssuer = true,

                ClockSkew = new TimeSpan(TimeSpan.Zero.Seconds)
            };

            options.Audience = _issuer.Audience;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
                Configure(options);
        }
    }
}