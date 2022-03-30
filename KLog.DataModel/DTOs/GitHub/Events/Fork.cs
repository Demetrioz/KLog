using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub.Events
{
    public class Fork : GitHubEvent
    {
        /// <summary>
        /// The created repository
        /// </summary>
        [JsonProperty("forkee")]
        public Repository Forkee { get; set; }
    }
}
