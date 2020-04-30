namespace MakersPortal.Core.Models
{
    public class IdentityProvider
    {
        public string Name { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public bool SkipValidation { get; set; } = false;
    }
}