using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    public class PullRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("html_url")]
        public string Url { get; set; }
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("locked")]
        public bool Locked { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("user")]
        public Owner User { get; set; }
        [JsonProperty("labels")]
        public List<Label> Labels { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? Created { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset? Updated { get; set; }
        [JsonProperty("closed_at")]
        public DateTimeOffset? Closed { get; set; }
        [JsonProperty("merged_at")]
        public DateTimeOffset? Merged { get; set; }
        [JsonProperty("assignee")]
        public Owner Assignee { get; set; }
        [JsonProperty("assignees")]
        public List<Owner> Assignees { get; set; }
        [JsonProperty("requested_reviewers")]
        public List<Owner> Reviewers { get; set; }
        [JsonProperty("repo")]
        public Repository Repo { get; set; }
        [JsonProperty("draft")]
        public bool Draft { get; set; }
    }
}
