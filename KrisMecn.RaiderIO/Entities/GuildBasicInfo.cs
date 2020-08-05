using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class GuildBasicInfo
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("realm")]
        public string Realm;
    }
}
