using System.Collections.Generic;
using MakersPortal.Core.Pocos;

namespace MakersPortal.Infrastructure.Options
{
    public class IdentityProvidersOptions
    {
        public IEnumerable<IdentityProvider> IdentityProviders { get; set; }
    }
}