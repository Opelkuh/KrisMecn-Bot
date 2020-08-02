using Newtonsoft.Json;

namespace KrisMecn.RaiderIO.Entities
{
    public class ErrorResponse
    {
        [JsonProperty("statusCode")]
        public int StatusCode;

        [JsonProperty("error")]
        public string ErrorCodeName;

        [JsonProperty("message")]
        public string Message;
    }
}
