using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace KrisMecn.RaiderIO.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Faction
    {
        [EnumMember(Value = "alliance")]
        Alliance,
        [EnumMember(Value = "horde")]
        Horde,
    }
}
