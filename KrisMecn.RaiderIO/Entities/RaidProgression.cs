using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class RaidProgression
    {
        [JsonProperty("summary")]
        /// <summary>
        /// Human-readable summary of the progression
        /// e.g. 4/10 H
        /// </summary>
        public string Summary;

        [JsonProperty("total_bosses")]
        /// <summary>
        /// Total bosses in the raid instance
        /// </summary>
        public int TotalBosses;

        [JsonProperty("normal_bosses_killed")]
        public int NormalBossesKilled;

        [JsonProperty("heroic_bosses_killed")]
        public int HeroicBossesKilled;

        [JsonProperty("mythic_bosses_killed")]
        public int MythicBossesKilled;
    }
}
