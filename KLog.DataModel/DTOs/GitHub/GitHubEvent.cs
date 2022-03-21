using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    // https://docs.github.com/en/developers/webhooks-and-events/webhooks/webhook-events-and-payloads
    public class GitHubEvent
    {
        /// <summary>
        /// The activity that triggered the event
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// The user that triggered the event
        /// </summary>
        [JsonProperty("sender")]
        public object Sender { get; set; }

        /// <summary>
        /// The repository where the event occured
        /// </summary>
        [JsonProperty("repository")]
        public Repository Repo { get; set; }

        /// <summary>
        /// The organization if the event was from an organization, or a repo
        /// within an organization
        /// </summary>
        [JsonProperty("organization")]
        public Organization Org { get; set; }
    }
}
