using System;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class RaidCurveInfo
    {
        [JsonProperty("raid")]
        public string RaidSlug;

        [JsonProperty("aotc")]
        public DateTime? AheadOfTheCurveRecieved;

        [JsonProperty("cutting_edge")]
        public DateTime? CuttingEdgeRecieved;
    }
}
