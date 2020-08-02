using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text.Json.Serialization;

namespace KrisMecn.Entities
{
    class Config
    {
        [JsonProperty("token")]
        internal string Token = "Your token..";

        [JsonProperty("googleApiKey")]
        internal string GoogleApiKey = "Your Google API key...";

        [JsonProperty("prefix")]
        public string Prefix = "!";

        [JsonProperty("activity")]
        public Activity Activity = new Activity();

        [JsonProperty("helpPrefix")]
        public string HelpPrefix = "";

        [JsonProperty("inviteLink")]
        public string InviteLink = "";

        public static Config LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            }
        }

        public void SaveToFile(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.Write(JsonConvert.SerializeObject(this));
            }
        }
    }

    class Activity
    {
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("status")]
        public UserStatus Status = UserStatus.Online;

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public ActivityType Type = ActivityType.Custom;

        [JsonProperty("text")]
        public string Text = "";

        [JsonPropertyName("prefix")]
        public string Prdel {get; set;}
    }
}
