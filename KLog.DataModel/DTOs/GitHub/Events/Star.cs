using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class Star : GitHubEvent
    {
        [JsonProperty("starred_at")]
        public DateTimeOffset StarredAt { get; set; }
    }
}
