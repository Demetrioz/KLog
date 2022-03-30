using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class LabelEvent : GitHubEvent
    {
        /// <summary>
        /// The label that was added
        /// </summary>
        [JsonProperty("label")]
        public Label Label { get; set; }
    }
}
