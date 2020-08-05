using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusScore
    {
        [JsonProperty("all")]
        public float All;

        [JsonProperty("dps")]
        public float DPS;

        [JsonProperty("healer")]
        public float Healer;

        [JsonProperty("tank")]
        public float Tank;

        [JsonProperty("spec_0")]
        public float Spec0;

        [JsonProperty("spec_1")]
        public float Spec1;

        [JsonProperty("spec_2")]
        public float Spec2;

        [JsonProperty("spec_3")]
        public float Spec3;
    }
}
