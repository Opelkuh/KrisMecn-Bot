using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class GearInfo
    {
        [JsonProperty("item_level_equipped")]
        public int ItemLevelEquipped;

        [JsonProperty("item_level_total")]
        public int ItemLevelTotal;

        [JsonProperty("artifact_traits")]
        public double ArtifactTraits;
    }
}
