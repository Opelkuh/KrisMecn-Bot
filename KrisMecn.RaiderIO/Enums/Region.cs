using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace KrisMecn.RaiderIO.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Region
    {
        [EnumMember(Value = "us")]
        US,
        [EnumMember(Value = "eu")]
        EU,
        [EnumMember(Value = "tw")]
        TW,
        [EnumMember(Value = "kr")]
        KR,
        [EnumMember(Value = "cn")]
        CN,
    }
}
