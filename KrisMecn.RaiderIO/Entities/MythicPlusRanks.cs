using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusRanks
    {
        [JsonProperty("overall")]
        public MythicPlusRank? Overall;

        [JsonProperty("tank")]
        public MythicPlusRank? Tank;

        [JsonProperty("healer")]
        public MythicPlusRank? Healer;

        [JsonProperty("dps")]
        public MythicPlusRank? DPS;

        [JsonProperty("class")]
        public MythicPlusRank? Class;

        [JsonProperty("class_tank")]
        public MythicPlusRank? ClassTank;

        [JsonProperty("class_healer")]
        public MythicPlusRank? ClassHealer;

        [JsonProperty("class_dps")]
        public MythicPlusRank? ClassDPS;
    }
}
