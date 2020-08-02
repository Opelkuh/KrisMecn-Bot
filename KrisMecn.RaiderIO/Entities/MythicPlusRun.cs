using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusRun
    {
        [JsonProperty("dungeon")]
        public int Dungeon;

        [JsonProperty("short_name")]
        /// <summary>
        /// Short name of the dungeon
        /// </summary>
        public int ShortName;

        [JsonProperty("mythic_level")]
        public int MythicLevel;

        [JsonProperty("completed_at")]
        public DateTime CompletedAt;

        [JsonProperty("clear_time_ms")]
        public long ClearTimeMS;

        [JsonProperty("num_keystone_upgrades")]
        public int NumKeystoneUpgrades;

        [JsonProperty("score")]
        public int Score;

        [JsonProperty("url")]
        public int URL;

        [JsonProperty("affixed")]
        public List<MythicPlusAffix>? Affixes;
    }
}
