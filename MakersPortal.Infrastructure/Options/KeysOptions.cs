using System.Reflection;

namespace MakersPortal.Infrastructure.Options
{
    public class KeysOptions
    {
        public Key this[string keyName] =>
            GetType().GetProperty(keyName,
                BindingFlags.Public | BindingFlags.Instance)?.GetValue(this, null) as Key;

        public Key Jwt { get; set; }

        public class Key
        {
            public string Public { get; set; }

            public string Private { get; set; }

            public string Kid { get; set; }

            public string Version { get; set; }
        }
    }
}