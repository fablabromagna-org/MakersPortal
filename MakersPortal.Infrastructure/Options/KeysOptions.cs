using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using static MakersPortal.Infrastructure.Options.KeysOptions;

namespace MakersPortal.Infrastructure.Options
{
    public class KeysOptions
    {
        public Key this[string keyName] => GetType().GetProperty(keyName, 
                BindingFlags.Public | BindingFlags.Instance)?.GetValue(typeof(Key), null) as Key;

        public Key Jwt;

        public class Key
        {
            public string Public { get; set; }

            public string Private { get; set; }

            public string Kid { get; set; }

            public string Version { get; set; }
        }
    }
}