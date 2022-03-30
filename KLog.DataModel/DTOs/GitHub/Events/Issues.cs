using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    /// <summary>
    /// Activity related to an issue
    /// </summary>
    public class Issues : GitHubEvent
    {
        /// <summary>
        /// The issue iteself
        /// </summary>
        [JsonProperty("issue")]
        public Issue Issue { get; set; }

        /// <summary>
        /// The user who was assigned / unassigned to the issue
        /// </summary>
        [JsonProperty("assignee")]
        public Owner Assignee { get; set; }
        
        /// <summary>
        /// The label that was added / removed to the issue
        /// </summary>
        [JsonProperty("label")]
        public Label Label { get; set; }
    }
}
