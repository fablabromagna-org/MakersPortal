using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace MakersPortal.WebApi.Options
{
    public class ConfigureForwardedHeadersOptions : IConfigureNamedOptions<ForwardedHeadersOptions>
    {
        public void Configure(ForwardedHeadersOptions options)
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        }

        public void Configure(string name, ForwardedHeadersOptions options)
        {
            Configure(options);
        }
    }
}