using System;
using System.Collections.Generic;
using System.Linq;
using MakersPortal.Core.Models;
using MakersPortal.Infrastructure.Options;
using MakersPortal.WebApi.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MakersPortal.WebApi.Options
{
    public class ConfigureAuthorizationOptions : IConfigureNamedOptions<AuthorizationOptions>
    {
        public void Configure(AuthorizationOptions options)
        {
            
          
        }

        public void Configure(string name, AuthorizationOptions options)
        {
            Configure(options);
        }
    }
}