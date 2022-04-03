using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    public class Pusher
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
