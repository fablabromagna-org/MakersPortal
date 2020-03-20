namespace MakersPortal.Core.Dtos.Configuration
{
    public class IdentityProviderDto
    {
        public string Name { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }
    }
}