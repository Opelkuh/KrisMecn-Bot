using System;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusAffix
    {
        [JsonProperty("id")]
        public int ID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("wowhead_url")]
        public Uri WowheadURL;
    }
}
