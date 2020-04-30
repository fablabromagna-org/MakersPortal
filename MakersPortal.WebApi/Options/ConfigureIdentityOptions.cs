using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MakersPortal.WebApi.Options
{
    public class ConfigureIdentityOptions : IConfigureNamedOptions<IdentityOptions>
    {
        public void Configure(IdentityOptions options)
        {
            options.User.RequireUniqueEmail = true;
        }

        public void Configure(string name, IdentityOptions options)
        {
            Configure(options);
        }
    }
}