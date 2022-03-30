using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class IssueComment : GitHubEvent
    {
        /// <summary>
        /// The changes to a comment if it was edited
        /// </summary>
        [JsonProperty("changes")]
        public object Changes { get; set; }

        /// <summary>
        /// The issue the comment belongs to
        /// </summary>
        [JsonProperty("issue")]
        public Issue Issue { get; set; }

        /// <summary>
        /// The comment itself
        /// </summary>
        [JsonProperty("comment")]
        public Comment Comment { get; set; }
    }
}
