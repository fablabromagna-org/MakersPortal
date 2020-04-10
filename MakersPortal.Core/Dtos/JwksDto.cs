using System.Collections.Generic;
using Newtonsoft.Json;

namespace MakersPortal.Core.Dtos
{
    public class JwksDto
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore,
            PropertyName = "keys", Required = Required.Default)]
        public IEnumerable<JwkDto> Keys { get; set; }
    }
}