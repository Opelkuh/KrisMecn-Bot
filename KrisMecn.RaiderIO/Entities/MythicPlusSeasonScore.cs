using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class MythicPlusSeasonScore
    {
        [JsonProperty("season")]
        public string SeasonSlug;

        [JsonProperty("scores")]
        public MythicPlusScore Score;
    }
}
