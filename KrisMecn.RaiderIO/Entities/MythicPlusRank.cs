using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusRank
    {
        [JsonProperty("world")]
        public int World;

        [JsonProperty("region")]
        public int Region;

        [JsonProperty("realm")]
        public int Realm;
    }
}
