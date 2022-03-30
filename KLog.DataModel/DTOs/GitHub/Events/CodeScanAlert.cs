using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    /// <summary>
    /// Triggers when there's activity related to code scanning alerts in a repository
    /// </summary>
    public class CodeScanAlert : GitHubEvent
    {
        /// <summary>
        /// The code scanning alert
        /// </summary>
        [JsonProperty("alert")]
        public object Alert { get; set; }

        /// <summary>
        /// The git reference of the code scanning alert
        /// </summary>
        [JsonProperty("ref")]
        public string Ref { get; set; }
    }
}
