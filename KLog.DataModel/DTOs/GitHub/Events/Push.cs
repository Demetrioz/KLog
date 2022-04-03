using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class Push : GitHubEvent
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }
        [JsonProperty("pusher")]
        public Pusher Pusher { get; set; }
    }
}
