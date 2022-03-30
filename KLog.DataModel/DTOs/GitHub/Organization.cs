using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    // https://docs.github.com/en/rest/reference/orgs#get-an-organization
    public class Organization
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("repos_url")]
        public string ReposUrl { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }
        [JsonProperty("blog")]
        public string Blog { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("public_repos")]
        public int PublicRepos { get; set; }
        [JsonProperty("followers")]
        public int Followers { get; set; }
    }
}
