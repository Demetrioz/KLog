using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    public class Owner
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("site_admin")]
        public bool Admin { get; set; }
    }
}
