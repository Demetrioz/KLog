using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class PullRequestEvent : GitHubEvent
    {
        /// <summary>
        /// The PR Number
        /// </summary>
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("pull_request")]
        public PullRequest PR { get; set; }
    }
}
