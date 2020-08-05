using System;
using System.Collections.Generic;
using KrisMecn.RaiderIO.Enums;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusAffixes
    {
        [JsonProperty("region")]
        public Region Region;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("leaderboard_url")]
        public Uri LeaderboardURL;

        [JsonProperty("affix_details")]
        public List<MythicPlusAffix> Affixes;
    }
}
