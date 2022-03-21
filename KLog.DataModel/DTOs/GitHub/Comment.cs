using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    // https://docs.github.com/en/rest/reference/commits#get-a-commit-comment
    public class Comment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("user")]
        public Owner User { get; set; }
    }
}
