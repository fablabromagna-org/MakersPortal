using System.Collections.Generic;
using System.Linq;
using MakersPortal.Core.Models;
using MakersPortal.Infrastructure.Options;
using MakersPortal.WebApi.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MakersPortal.WebApi.Options
{
    public class ConfigureAuthorizationOptions : IConfigureNamedOptions<AuthorizationOptions>
    {
        private readonly IEnumerable<IdentityProvider> _identityProviders;
        
        public ConfigureAuthorizationOptions(IOptions<IdentityProvidersOptions> identityProvidersOptions)
        {
            _identityProviders = identityProvidersOptions.Value.IdentityProviders;
        }
        
        public void Configure(AuthorizationOptions options)
        {
            options.AddPolicy(PoliciesConstants.EXTERNAL_IDP_ONLY_POLICY, ConfigureExternalIdpOnlyPolicy);
        }

        private void ConfigureExternalIdpOnlyPolicy(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAuthenticatedUser();
            
            builder.AuthenticationSchemes.Clear();
            builder.AddAuthenticationSchemes(_identityProviders.Select(p => p.Name).ToArray());
        }

        public void Configure(string name, AuthorizationOptions options)
        {
            Configure(options);
        }
    }
}