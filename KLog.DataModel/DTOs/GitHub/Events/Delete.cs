using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    /// <summary>
    /// Triggers when a branch or tag is deleted
    /// </summary>
    public class Delete : GitHubEvent
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        /// <summary>
        /// Type of object created - either branch or tag
        /// </summary>
        [JsonProperty("ref_type")]
        public string RefType { get; set; }

        /// <summary>
        /// User that triggered the event
        /// </summary>
        [JsonProperty("sender")]
        public Owner Sender { get; set; }
    }
}
