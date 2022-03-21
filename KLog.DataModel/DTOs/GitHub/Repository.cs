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
    }
}
