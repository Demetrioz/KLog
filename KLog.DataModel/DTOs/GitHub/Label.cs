using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    public class Label
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("url")]
        public string URL { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("default")]
        public bool Default { get; set; }
    }
}
