using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    public class Issue
    {
        [JsonProperty("url")]
        public string URL { get; set; }
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("user")]
        public Owner User { get; set; }
        [JsonProperty("labels")]
        public List<Label> Labels { get; set; }
        [JsonProperty("assignee")]
        public Owner Assignee { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset Created { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset Updated { get; set; }
        [JsonProperty("closed_at")]
        public DateTimeOffset Closed { get; set; }
        [JsonProperty("repository")]
        public Repository Repo { get; set; }
    }
}
