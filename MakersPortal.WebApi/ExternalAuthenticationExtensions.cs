using System;
using System.Collections.Generic;
using MakersPortal.Core.Pocos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi
{
    public static class ExternalAuthenticationExtensions
    {
        public static AuthenticationBuilder AddExternalIdentityProviders(this AuthenticationBuilder builder,
            IEnumerable<IdentityProvider> identityProviders)
        {
            foreach (var identityProvider in identityProviders)
            {
                builder.AddJwtBearer(identityProvider.Name,options => ConfigureOptions(options, identityProvider));
            }

            return builder;
        }

        private static void ConfigureOptions(JwtBearerOptions options, IdentityProvider identityProvider)
        {
            bool performValidation = !identityProvider.SkipValidation;

            options.Audience = identityProvider.Audience;

            if (performValidation)
                options.Authority = identityProvider.Issuer;

            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = identityProvider.Issuer,
                ValidAudience = identityProvider.Audience,
                ValidateAudience = performValidation,
                ValidateIssuer = performValidation,
                ValidateLifetime = performValidation,
                ValidateTokenReplay = performValidation,
                RequireAudience = performValidation,
                RequireExpirationTime = performValidation,
                RequireSignedTokens = performValidation,
                ValidateIssuerSigningKey = performValidation,

                ClockSkew = new TimeSpan(TimeSpan.Zero.Seconds)
            };
        }
    }
}