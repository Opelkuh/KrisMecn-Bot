using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace KrisMecn.RaiderIO.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Locale
    {
        [EnumMember(Value = "en")]
        EN,
        [EnumMember(Value = "ru")]
        RU,
        [EnumMember(Value = "ko")]
        KO,
        [EnumMember(Value = "cn")]
        CN,
        [EnumMember(Value = "pt")]
        PT,
        [EnumMember(Value = "it")]
        IT,
        [EnumMember(Value = "fr")]
        FR,
        [EnumMember(Value = "es")]
        ES,
        [EnumMember(Value = "de")]
        DE,
        [EnumMember(Value = "tw")]
        TW,
    }
}
