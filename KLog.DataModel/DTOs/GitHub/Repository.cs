using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    // https://docs.github.com/en/rest/reference/repos#get-a-repository
    public class Repository
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("owner")]
        public Owner Owner { get; set; }
        [JsonProperty("private")]
        public bool Private { get; set; }
        [JsonProperty("html_url")]
        public string HTMLUrl { get; set; }
        [JsonProperty("default_branch")]
        public string DefaultBranch { get; set; }
        [JsonProperty("forks")]
        public int Forks { get; set; }
        [JsonProperty("watchers")]
        public int Watchers { get; set; }
        [JsonProperty("open_issues")]
        public int OpenIssues { get; set; }
    }
}
