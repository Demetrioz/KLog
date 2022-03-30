using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class Ping : GitHubEvent
    {
        /// <summary>
        /// Random string of GitHub zen
        /// </summary>
        [JsonProperty("zen")]
        public string Zen { get; set; }

        /// <summary>
        /// ID of the webhook that triggered the ping
        /// </summary>
        [JsonProperty("hook_id")]
        public int HookId { get; set; }
    }
}
